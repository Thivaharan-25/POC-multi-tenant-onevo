using System.Text.Json;
using System.Text.Json.Serialization;
using OnevoHr.Api.DTOs.Admin;
using OnevoHr.Api.Models.Auth;
using OnevoHr.Api.Models.Leave;
using OnevoHr.Api.Models.OrgStructure;
using OnevoHr.Api.Models.Templates;
using OnevoHr.Api.Repositories.Interfaces;
using OnevoHr.Api.Services.Interfaces;

namespace OnevoHr.Api.Services.Implementations;

/// <summary>
/// Applies global configuration templates to a tenant. Template payloads use
/// template-local keys; application resolves them into real tenant records.
/// Global templates are never mutated; each application is recorded.
/// </summary>
public class TemplateApplicationService : ITemplateApplicationService
{
    private readonly ITemplateRepository _templates;
    private readonly IOrgRepository _org;
    private readonly ILeaveRepository _leave;
    private readonly IRoleRepository _roles;
    private readonly IPermissionRepository _permissions;

    public TemplateApplicationService(
        ITemplateRepository templates,
        IOrgRepository org,
        ILeaveRepository leave,
        IRoleRepository roles,
        IPermissionRepository permissions)
    {
        _templates = templates;
        _org = org;
        _leave = leave;
        _roles = roles;
        _permissions = permissions;
    }

    public async Task<List<ConfigurationTemplateDto>> GetConfigurationTemplatesAsync()
    {
        var templates = await _templates.GetConfigurationTemplatesAsync();
        return templates
            .Select(t => new ConfigurationTemplateDto(
                t.Id, t.TemplateKey, t.TemplateType, t.Name, t.Version,
                t.EmployeeRangeMin, t.EmployeeRangeMax, t.IsActive))
            .ToList();
    }

    public async Task<List<RoleTemplateDto>> GetRoleTemplatesAsync()
    {
        var templates = await _templates.GetRoleTemplatesAsync();
        return templates
            .Select(t => new RoleTemplateDto(
                t.Id, t.Name, t.Description, t.IsSystem, t.IsActive,
                (JsonSerializer.Deserialize<List<string>>(t.PermissionCodesJson) ?? new List<string>())
                    .OrderBy(k => k)
                    .ToList()))
            .ToList();
    }

    public async Task ApplyTemplatesForEmployeeCountAsync(Guid tenantId, int employeeCount, Guid? appliedByPlatformUserId)
    {
        var positionTemplate = await _templates.FindByTypeAndEmployeeCountAsync("position_template", employeeCount);
        if (positionTemplate is not null)
        {
            await ApplyConfigurationTemplateAsync(tenantId, positionTemplate.Id, appliedByPlatformUserId);
        }
    }

    public async Task<bool> ApplyConfigurationTemplateAsync(Guid tenantId, Guid configurationTemplateId, Guid? appliedByPlatformUserId)
    {
        var template = await _templates.GetConfigurationTemplateByIdAsync(configurationTemplateId);
        if (template is null || !template.IsActive)
        {
            return false;
        }

        switch (template.TemplateType)
        {
            case "position_template":
                await ApplyPositionTemplateAsync(tenantId, template);
                break;
            case "leave_policy":
                await ApplyLeavePolicyTemplateAsync(tenantId, template);
                break;
        }

        await _templates.AddApplicationAsync(new TenantConfigurationTemplateApplication
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            ConfigurationTemplateId = template.Id,
            TemplateType = template.TemplateType,
            AppliedVersion = template.Version,
            Status = "applied",
            AppliedAtUtc = DateTime.UtcNow,
            AppliedByPlatformUserId = appliedByPlatformUserId
        });
        await _templates.SaveChangesAsync();
        return true;
    }

    private async Task ApplyPositionTemplateAsync(Guid tenantId, ConfigurationTemplate template)
    {
        var payload = JsonSerializer.Deserialize<PositionTemplatePayload>(template.PayloadJson);
        if (payload?.Positions is null || payload.Positions.Count == 0)
        {
            return;
        }

        var now = DateTime.UtcNow;
        var legalEntity = (await _org.GetLegalEntitiesAsync(tenantId)).FirstOrDefault();
        if (legalEntity is null)
        {
            legalEntity = new LegalEntity
            {
                Id = Guid.NewGuid(),
                TenantId = tenantId,
                Name = "Main Legal Entity",
                Code = "MAIN",
                Status = "active",
                CreatedAtUtc = now
            };
            await _org.AddLegalEntityAsync(legalEntity);
        }

        var departments = (await _org.GetDepartmentsAsync(tenantId, null))
            .Where(d => d.LegalEntityId == legalEntity.Id)
            .ToDictionary(d => d.Name, d => d);

        // First pass: create departments and positions.
        var createdPositions = new Dictionary<string, Position>();
        foreach (var item in payload.Positions)
        {
            if (!departments.TryGetValue(item.DepartmentName, out var department))
            {
                department = new Department
                {
                    Id = Guid.NewGuid(),
                    TenantId = tenantId,
                    LegalEntityId = legalEntity.Id,
                    Name = item.DepartmentName,
                    Code = ToCode(item.DepartmentName),
                    Status = "active",
                    CreatedAtUtc = now
                };
                await _org.AddDepartmentAsync(department);
                departments[item.DepartmentName] = department;
            }

            Guid? suggestedRoleId = null;
            if (Guid.TryParse(item.LinkedRoleTemplateId, out var roleTemplateId))
            {
                suggestedRoleId = await MaterializeRoleTemplateAsync(tenantId, roleTemplateId, now);
            }

            var position = new Position
            {
                Id = Guid.NewGuid(),
                TenantId = tenantId,
                LegalEntityId = legalEntity.Id,
                DepartmentId = department.Id,
                Name = item.PositionName,
                Code = ToCode(item.PositionKey),
                Capacity = item.Capacity,
                PositionType = item.PositionType,
                Status = "active",
                SuggestedRoleId = suggestedRoleId,
                CreatedAtUtc = now
            };
            await _org.AddPositionAsync(position);
            createdPositions[item.PositionKey] = position;
        }

        // Second pass: resolve reports_to_position_key to real position ids.
        foreach (var item in payload.Positions)
        {
            if (item.ReportsToPositionKey is not null &&
                createdPositions.TryGetValue(item.ReportsToPositionKey, out var manager))
            {
                var position = createdPositions[item.PositionKey];
                position.ReportsToPositionId = manager.Id;

                await _org.AddPositionReportingHistoryAsync(new PositionReportingHistory
                {
                    Id = Guid.NewGuid(),
                    TenantId = tenantId,
                    PositionId = position.Id,
                    ReportsToPositionId = manager.Id,
                    EffectiveFromUtc = now
                });
            }
        }
    }

    private async Task<Guid?> MaterializeRoleTemplateAsync(Guid tenantId, Guid roleTemplateId, DateTime now)
    {
        var roleTemplate = await _templates.GetRoleTemplateByIdAsync(roleTemplateId);
        if (roleTemplate is null)
        {
            return null;
        }

        var existing = await _roles.GetByNameAsync(tenantId, roleTemplate.Name);
        if (existing is not null)
        {
            return existing.Id;
        }

        var role = new Role
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            Name = roleTemplate.Name,
            Description = roleTemplate.Description,
            IsSystemRole = false,
            SourceTemplateId = roleTemplate.Id,
            CreatedAtUtc = now
        };
        await _roles.AddAsync(role);

        var permissionCodes = JsonSerializer.Deserialize<List<string>>(roleTemplate.PermissionCodesJson) ?? new List<string>();
        var allPermissions = await _permissions.GetAllPermissionsAsync();
        var permissionIdsByCode = allPermissions.ToDictionary(p => p.Code, p => p.Id);

        foreach (var code in permissionCodes)
        {
            if (permissionIdsByCode.TryGetValue(code, out var permissionId))
            {
                await _roles.AddRolePermissionAsync(new RolePermission
                {
                    Id = Guid.NewGuid(),
                    RoleId = role.Id,
                    PermissionId = permissionId
                });
            }
        }

        return role.Id;
    }

    private async Task ApplyLeavePolicyTemplateAsync(Guid tenantId, ConfigurationTemplate template)
    {
        var payload = JsonSerializer.Deserialize<LeavePolicyTemplatePayload>(template.PayloadJson);
        if (payload?.LeaveTypes is null)
        {
            return;
        }

        var now = DateTime.UtcNow;
        var departments = await _org.GetDepartmentsAsync(tenantId, null);

        foreach (var item in payload.LeaveTypes)
        {
            var leaveType = await _leave.GetLeaveTypeByCodeAsync(tenantId, item.Code);
            if (leaveType is null)
            {
                leaveType = new LeaveType
                {
                    Id = Guid.NewGuid(),
                    TenantId = tenantId,
                    Code = item.Code,
                    Name = item.Name,
                    IsPaid = true,
                    RequiresApproval = item.RequiresApproval,
                    IsActive = true
                };
                await _leave.AddLeaveTypeAsync(leaveType);
            }

            var policy = new LeavePolicy
            {
                Id = Guid.NewGuid(),
                TenantId = tenantId,
                LeaveTypeId = leaveType.Id,
                EntitlementDays = item.EntitlementDays,
                CarryForwardAllowed = item.CarryForwardAllowed,
                CarryForwardLimit = item.CarryForwardLimit,
                RequiresApproval = item.RequiresApproval,
                CreatedAtUtc = now
            };
            await _leave.AddLeavePolicyAsync(policy);

            if (item.AssignmentScope == "department" && item.DepartmentTemplateKeys is not null)
            {
                foreach (var departmentName in item.DepartmentTemplateKeys)
                {
                    var department = departments.FirstOrDefault(d => d.Name == departmentName);
                    if (department is not null)
                    {
                        await _leave.AddLeavePolicyAssignmentAsync(new LeavePolicyAssignment
                        {
                            Id = Guid.NewGuid(),
                            TenantId = tenantId,
                            LeavePolicyId = policy.Id,
                            AssignmentScope = "department",
                            DepartmentId = department.Id
                        });
                    }
                }
            }
            else
            {
                await _leave.AddLeavePolicyAssignmentAsync(new LeavePolicyAssignment
                {
                    Id = Guid.NewGuid(),
                    TenantId = tenantId,
                    LeavePolicyId = policy.Id,
                    AssignmentScope = "tenant"
                });
            }
        }
    }

    private static string ToCode(string value)
    {
        var upper = value.ToUpperInvariant();
        var codeChars = upper.Select(c => char.IsLetterOrDigit(c) ? c : '_').ToArray();
        return new string(codeChars);
    }

    private sealed class PositionTemplatePayload
    {
        [JsonPropertyName("positions")]
        public List<PositionTemplateItem> Positions { get; set; } = new();
    }

    private sealed class PositionTemplateItem
    {
        [JsonPropertyName("position_key")]
        public string PositionKey { get; set; } = string.Empty;

        [JsonPropertyName("position_name")]
        public string PositionName { get; set; } = string.Empty;

        [JsonPropertyName("department_name")]
        public string DepartmentName { get; set; } = string.Empty;

        [JsonPropertyName("reports_to_position_key")]
        public string? ReportsToPositionKey { get; set; }

        [JsonPropertyName("capacity")]
        public int Capacity { get; set; } = 1;

        [JsonPropertyName("position_type")]
        public string PositionType { get; set; } = "unique";

        [JsonPropertyName("linked_role_template_id")]
        public string? LinkedRoleTemplateId { get; set; }
    }

    private sealed class LeavePolicyTemplatePayload
    {
        [JsonPropertyName("leave_types")]
        public List<LeavePolicyTemplateItem> LeaveTypes { get; set; } = new();
    }

    private sealed class LeavePolicyTemplateItem
    {
        [JsonPropertyName("code")]
        public string Code { get; set; } = string.Empty;

        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("entitlement_days")]
        public decimal EntitlementDays { get; set; }

        [JsonPropertyName("requires_approval")]
        public bool RequiresApproval { get; set; }

        [JsonPropertyName("carry_forward_allowed")]
        public bool CarryForwardAllowed { get; set; }

        [JsonPropertyName("carry_forward_limit")]
        public decimal CarryForwardLimit { get; set; }

        [JsonPropertyName("assignment_scope")]
        public string AssignmentScope { get; set; } = "tenant";

        [JsonPropertyName("department_template_keys")]
        public List<string>? DepartmentTemplateKeys { get; set; }

        [JsonPropertyName("position_template_keys")]
        public List<string>? PositionTemplateKeys { get; set; }
    }
}
