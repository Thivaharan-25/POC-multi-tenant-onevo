using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using OnevoHr.Api.DTOs;
using OnevoHr.Api.DTOs.Onboarding;
using OnevoHr.Api.Options;
using OnevoHr.Api.Models.Auth;
using OnevoHr.Api.Models.Employees;
using OnevoHr.Api.Models.Generated;
using OnevoHr.Api.Models.Notifications;
using OnevoHr.Api.Models.OrgStructure;
using OnevoHr.Api.Repositories.Interfaces;
using OnevoHr.Api.Services.Interfaces;

namespace OnevoHr.Api.Services.Implementations;

public class OnboardingService : IOnboardingService
{
    private const string DraftStatusDraft = "draft";
    private const string DraftStatusCompleted = "completed";
    private const string ReasonSavedManually = "saved_manually";
    private const string ReasonWaitingForSeat = "waiting_for_seat";
    private const string ReasonWaitingForPositionApproval = "waiting_for_position_approval";
    private const string ActionRequestSeatIncrease = "request_seat_increase";
    private const string ActionSubmitPositionApproval = "submit_position_approval";

    private static readonly JsonSerializerOptions TaskJsonOptions = new JsonSerializerOptions
    {
        PropertyNameCaseInsensitive = true
    };

    private readonly IOnboardingRepository _onboardingRepository;
    private readonly IUserRepository _userRepository;
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IOrgRepository _orgRepository;
    private readonly ISubscriptionRepository _subscriptionRepository;
    private readonly IOutboxRepository _outboxRepository;
    private readonly IEmailDeliveryLogRepository _emailLogRepository;
    private readonly INotificationChannelRepository _notificationChannelRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly EmailOptions _emailOptions;
    private readonly IHostEnvironment _environment;

    public OnboardingService(
        IOnboardingRepository onboardingRepository,
        IUserRepository userRepository,
        IEmployeeRepository employeeRepository,
        IOrgRepository orgRepository,
        ISubscriptionRepository subscriptionRepository,
        IOutboxRepository outboxRepository,
        IEmailDeliveryLogRepository emailLogRepository,
        INotificationChannelRepository notificationChannelRepository,
        ICurrentUserService currentUserService,
        IOptions<EmailOptions> emailOptions,
        IHostEnvironment environment)
    {
        _onboardingRepository = onboardingRepository;
        _userRepository = userRepository;
        _employeeRepository = employeeRepository;
        _orgRepository = orgRepository;
        _subscriptionRepository = subscriptionRepository;
        _outboxRepository = outboxRepository;
        _emailLogRepository = emailLogRepository;
        _notificationChannelRepository = notificationChannelRepository;
        _currentUserService = currentUserService;
        _emailOptions = emailOptions.Value;
        _environment = environment;
    }

    // ------------------------------------------------------------------
    // Draft save / resume / checklist
    // ------------------------------------------------------------------

    public async Task<SaveDraftResult> SaveDraftAsync(SaveOnboardingDraftRequest request, CancellationToken ct)
    {
        var tenantId = GetCurrentTenantId();
        var userId = GetCurrentUserId();

        if (request.EditedTasksJson != null && !IsJsonArray(request.EditedTasksJson))
        {
            return new SaveDraftResult
            {
                Error = BuildIssue("ONB_TASKS_JSON_INVALID", "editedTasksJson must be a valid JSON array.", "editedTasksJson")
            };
        }

        var existingDraft = await _onboardingRepository.GetDraftByEmailAsync(tenantId, request.WorkEmail);

        if (existingDraft != null)
        {
            existingDraft.EmployeeName = request.EmployeeName;
            existingDraft.LegalEntityId = request.LegalEntityId;
            existingDraft.DepartmentId = request.DepartmentId;
            existingDraft.PositionId = request.PositionId;
            existingDraft.EmploymentType = request.EmploymentType;
            existingDraft.StartDate = request.StartDate;
            existingDraft.EmployeeNumber = request.EmployeeNumber;
            existingDraft.ScheduleId = request.ScheduleId;
            existingDraft.LastSavedStep = request.LastSavedStep;

            if (request.SelectedTemplateId.HasValue)
            {
                existingDraft.SelectedTemplateId = request.SelectedTemplateId;
            }
            if (request.EditedTasksJson != null)
            {
                existingDraft.EditedTasksJson = request.EditedTasksJson;
            }

            existingDraft.UpdatedAtUtc = DateTime.UtcNow;

            await _onboardingRepository.SaveChangesAsync();
            return new SaveDraftResult { DraftId = existingDraft.Id };
        }

        var newDraft = new OnboardingDraft
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            EmployeeName = request.EmployeeName,
            WorkEmail = request.WorkEmail,
            LegalEntityId = request.LegalEntityId,
            DepartmentId = request.DepartmentId,
            PositionId = request.PositionId,
            EmploymentType = request.EmploymentType,
            StartDate = request.StartDate,
            EmployeeNumber = request.EmployeeNumber,
            ScheduleId = request.ScheduleId,
            SelectedTemplateId = request.SelectedTemplateId,
            EditedTasksJson = request.EditedTasksJson ?? "[]",
            Status = DraftStatusDraft,
            DraftReason = ReasonSavedManually,
            LastSavedStep = request.LastSavedStep,
            StartedById = userId,
            CreatedAtUtc = DateTime.UtcNow,
            UpdatedAtUtc = DateTime.UtcNow
        };

        await _onboardingRepository.AddDraftAsync(newDraft);
        await _onboardingRepository.SaveChangesAsync();

        return new SaveDraftResult { DraftId = newDraft.Id };
    }

    public async Task<OnboardingDraft?> GetDraftAsync(Guid id, CancellationToken ct)
    {
        var tenantId = GetCurrentTenantId();
        return await _onboardingRepository.GetDraftByIdAsync(tenantId, id);
    }

    public async Task<DraftActionResponse> UpdateChecklistDraftAsync(Guid id, UpdateChecklistDraftRequest request, CancellationToken ct)
    {
        var tenantId = GetCurrentTenantId();
        var draft = await _onboardingRepository.GetDraftByIdAsync(tenantId, id);

        if (draft == null)
        {
            return new DraftActionResponse { Status = "not_found" };
        }
        if (draft.Status != DraftStatusDraft)
        {
            return new DraftActionResponse { Status = "invalid_state", Message = "Draft is not in draft state." };
        }
        if (!IsJsonArray(request.EditedTasksJson))
        {
            return new DraftActionResponse { Status = "invalid", Message = "editedTasksJson must be a valid JSON array." };
        }

        draft.SelectedTemplateId = request.SelectedTemplateId;
        draft.EditedTasksJson = request.EditedTasksJson;
        draft.LastSavedStep = "checklist_review";
        draft.UpdatedAtUtc = DateTime.UtcNow;

        await _onboardingRepository.SaveChangesAsync();
        return new DraftActionResponse { Status = "ok" };
    }

    // ------------------------------------------------------------------
    // Checklist templates and my-drafts (read-only)
    // ------------------------------------------------------------------

    public async Task<ChecklistTemplateListResponse> GetChecklistTemplatesAsync(Guid? departmentId, CancellationToken ct)
    {
        var tenantId = GetCurrentTenantId();

        var templates = await _onboardingRepository.GetChecklistTemplatesAsync(tenantId);
        var summaries = templates
            .Select(t => new ChecklistTemplateSummaryDto(t.Id, t.Name, t.TemplateType, t.DepartmentId, t.IsActive))
            .ToList();

        // Recommended template: department-specific match first, then company-wide
        // fallback. Position-specific matching is deferred — ChecklistTemplate has
        // no PositionId column yet (see plan Global Constraints).
        ChecklistTemplate? recommended = null;
        if (departmentId.HasValue)
        {
            recommended = templates.FirstOrDefault(t => t.DepartmentId == departmentId.Value);
        }
        if (recommended == null)
        {
            recommended = templates.FirstOrDefault(t => t.DepartmentId == null);
        }

        return new ChecklistTemplateListResponse(summaries, recommended?.Id);
    }

    public async Task<ChecklistTemplateDetailDto?> GetChecklistTemplateDetailAsync(Guid templateId, CancellationToken ct)
    {
        var tenantId = GetCurrentTenantId();

        var template = await _onboardingRepository.GetChecklistTemplateAsync(tenantId, templateId);
        if (template == null)
        {
            return null;
        }

        var parsedTasks = TryParseTasks(template.TasksJson) ?? new List<DraftTaskDefinition>();
        var tasks = parsedTasks
            .Where(t => !string.IsNullOrWhiteSpace(t.Title))
            .Select(t => new ChecklistTemplateTaskDto(
                t.Title!, t.OwnerType, t.Sequence, t.IsRequired == true, t.IsLocked == true))
            .ToList();

        return new ChecklistTemplateDetailDto(
            template.Id, template.Name, template.TemplateType, template.DepartmentId, template.IsActive, tasks);
    }

    public async Task<List<MyDraftSummaryDto>> GetMyDraftsAsync(CancellationToken ct)
    {
        var tenantId = GetCurrentTenantId();
        var userId = GetCurrentUserId();

        var drafts = await _onboardingRepository.GetDraftsByStartedByAsync(tenantId, userId);
        return drafts
            .Select(d => new MyDraftSummaryDto(d.Id, d.EmployeeName, d.WorkEmail, d.LastSavedStep, d.DraftReason, d.UpdatedAtUtc))
            .ToList();
    }

    // ------------------------------------------------------------------
    // Pre-invite validation
    // ------------------------------------------------------------------

    public async Task<DraftValidationResult?> ValidateDraftAsync(Guid id, CancellationToken ct)
    {
        var tenantId = GetCurrentTenantId();
        var draft = await _onboardingRepository.GetDraftByIdAsync(tenantId, id);

        if (draft == null)
        {
            return null;
        }

        var result = await RunPreInviteValidationAsync(draft, tenantId);

        // Surface upcoming blockers as warnings so HR sees them before send-invite.
        var seat = await CheckSeatAvailabilityAsync(tenantId);
        if (seat.Warning != null)
        {
            result.Warnings.Add(seat.Warning);
        }
        if (!seat.Available)
        {
            result.Warnings.Add(BuildIssue("ONB_NO_SEAT", "No employee seat is available. Send-invite will block this draft.", null));
            result.Actions.Add(ActionRequestSeatIncrease);
        }

        var approvalRequired = await IsPositionApprovalRequiredAsync(tenantId, draft.PositionId);
        if (approvalRequired)
        {
            result.Warnings.Add(BuildIssue(
                "ONB_POSITION_APPROVAL_REQUIRED",
                "The selected position is sensitive and requires approval. Send-invite will block this draft.",
                "positionId"));
            result.Actions.Add(ActionSubmitPositionApproval);
        }

        return result;
    }

    private async Task<DraftValidationResult> RunPreInviteValidationAsync(OnboardingDraft draft, Guid tenantId)
    {
        var result = new DraftValidationResult();

        // 1. Legal entity (Company). employees.legal_entity_id is a required FK,
        //    so a draft cannot be finalized without one.
        if (!draft.LegalEntityId.HasValue)
        {
            result.Errors.Add(BuildIssue("ONB_LEGAL_ENTITY_REQUIRED", "A company/legal entity is required.", "legalEntityId"));
        }
        else
        {
            var legalEntity = await _orgRepository.GetLegalEntityByIdAsync(draft.LegalEntityId.Value);
            if (legalEntity == null || legalEntity.TenantId != tenantId)
            {
                result.Errors.Add(BuildIssue("ONB_LEGAL_ENTITY_INVALID", "The selected company/legal entity does not exist in this tenant.", "legalEntityId"));
            }
        }

        // 2. Department belongs to tenant and to the selected legal entity.
        if (draft.DepartmentId.HasValue)
        {
            var department = await _orgRepository.GetDepartmentByIdAsync(draft.DepartmentId.Value);
            if (department == null || department.TenantId != tenantId)
            {
                result.Errors.Add(BuildIssue("ONB_DEPARTMENT_INVALID", "The selected department does not exist in this tenant.", "departmentId"));
            }
            else if (draft.LegalEntityId.HasValue && department.LegalEntityId != draft.LegalEntityId.Value)
            {
                result.Errors.Add(BuildIssue("ONB_DEPARTMENT_WRONG_COMPANY", "The selected department belongs to a different company.", "departmentId"));
            }
        }

        // 3. Position belongs to tenant and to the selected department/legal entity.
        if (draft.PositionId.HasValue)
        {
            var position = await _orgRepository.GetPositionByIdAsync(draft.PositionId.Value);
            if (position == null || position.TenantId != tenantId)
            {
                result.Errors.Add(BuildIssue("ONB_POSITION_INVALID", "The selected position does not exist in this tenant.", "positionId"));
            }
            else
            {
                if (draft.LegalEntityId.HasValue && position.LegalEntityId != draft.LegalEntityId.Value)
                {
                    result.Errors.Add(BuildIssue("ONB_POSITION_WRONG_COMPANY", "The selected position belongs to a different company.", "positionId"));
                }
                if (draft.DepartmentId.HasValue && position.DepartmentId != draft.DepartmentId.Value)
                {
                    result.Errors.Add(BuildIssue("ONB_POSITION_WRONG_DEPARTMENT", "The selected position belongs to a different department.", "positionId"));
                }
            }
        }

        // 4. Work email must be unique against users and employees in this tenant.
        var existingUser = await _userRepository.GetByEmailAsync(tenantId, draft.WorkEmail);
        if (existingUser != null)
        {
            result.Errors.Add(BuildIssue("ONB_WORK_EMAIL_TAKEN", "A user with this work email already exists.", "workEmail"));
        }
        var existingEmployee = await _employeeRepository.GetByWorkEmailAsync(tenantId, draft.WorkEmail);
        if (existingEmployee != null)
        {
            result.Errors.Add(BuildIssue("ONB_WORK_EMAIL_TAKEN_EMPLOYEE", "An employee with this work email already exists.", "workEmail"));
        }

        // 5. Employee number must be unique when supplied.
        if (!string.IsNullOrWhiteSpace(draft.EmployeeNumber))
        {
            var numberOwner = await _employeeRepository.GetByEmployeeNumberAsync(tenantId, draft.EmployeeNumber);
            if (numberOwner != null)
            {
                result.Errors.Add(BuildIssue("ONB_EMPLOYEE_NUMBER_TAKEN", "This employee number is already in use.", "employeeNumber"));
            }
        }

        // 6. Schedule must exist in this tenant when supplied.
        if (draft.ScheduleId.HasValue)
        {
            var schedule = await _onboardingRepository.GetWorkScheduleAsync(tenantId, draft.ScheduleId.Value);
            if (schedule == null)
            {
                result.Errors.Add(BuildIssue("ONB_SCHEDULE_INVALID", "The selected work schedule does not exist in this tenant.", "scheduleId"));
            }
        }

        // 7. Checklist template must exist when selected, and edited tasks must be valid JSON.
        ChecklistTemplate? template = null;
        if (draft.SelectedTemplateId.HasValue)
        {
            template = await _onboardingRepository.GetChecklistTemplateAsync(tenantId, draft.SelectedTemplateId.Value);
            if (template == null)
            {
                result.Errors.Add(BuildIssue("ONB_TEMPLATE_INVALID", "The selected checklist template does not exist in this tenant.", "selectedTemplateId"));
            }
        }

        List<DraftTaskDefinition>? editedTasks = TryParseTasks(draft.EditedTasksJson);
        if (editedTasks == null)
        {
            result.Errors.Add(BuildIssue("ONB_TASKS_JSON_INVALID", "The edited checklist tasks are not a valid JSON array.", "editedTasksJson"));
        }

        // 8. Required/locked template tasks must not be removed. The template's
        //    tasks_json supports isRequired/isLocked flags; matching is by title.
        if (template != null && editedTasks != null)
        {
            var templateTasks = TryParseTasks(template.TasksJson);
            if (templateTasks != null)
            {
                foreach (var templateTask in templateTasks)
                {
                    var mustBeKept = templateTask.IsRequired == true || templateTask.IsLocked == true;
                    if (!mustBeKept)
                    {
                        continue;
                    }

                    var stillPresent = editedTasks.Exists(t =>
                        string.Equals(t.Title, templateTask.Title, StringComparison.OrdinalIgnoreCase));
                    if (!stillPresent)
                    {
                        result.Errors.Add(BuildIssue(
                            "ONB_REQUIRED_TASK_REMOVED",
                            $"Required checklist task '{templateTask.Title}' cannot be removed.",
                            "editedTasksJson"));
                    }
                }
            }
        }

        result.IsValid = result.Errors.Count == 0;
        return result;
    }

    // ------------------------------------------------------------------
    // Seat availability and sensitive position approval
    // ------------------------------------------------------------------

    private sealed class SeatCheckResult
    {
        public bool Available { get; set; }
        public ValidationIssue? Warning { get; set; }
    }

    private async Task<SeatCheckResult> CheckSeatAvailabilityAsync(Guid tenantId)
    {
        var limits = await _subscriptionRepository.GetResourceLimitsAsync(tenantId);

        if (limits.Count == 0)
        {
            // No TenantResourceLimit row is seeded for this tenant. There is no
            // other canonical seat-limit model in this repo, so we do not block,
            // but we surface it instead of silently assuming a seat exists.
            return new SeatCheckResult
            {
                Available = true,
                Warning = BuildIssue("ONB_SEAT_LIMIT_NOT_CONFIGURED", "No employee seat limit is configured for this tenant; the seat check was skipped.", null)
            };
        }

        // GetResourceLimitsAsync returns newest first; the latest row wins.
        var latestLimit = limits[0];
        if (!latestLimit.EmployeeLimit.HasValue)
        {
            // Null means unlimited seats on the current plan.
            return new SeatCheckResult { Available = true };
        }

        var occupiedSeats = await _employeeRepository.CountOnboardingAndActiveAsync(tenantId);
        var available = occupiedSeats < latestLimit.EmployeeLimit.Value;
        return new SeatCheckResult { Available = available };
    }

    private async Task<bool> IsPositionApprovalRequiredAsync(Guid tenantId, Guid? positionId)
    {
        if (!positionId.HasValue)
        {
            return false;
        }

        // position_access_templates carries the canonical requires_approval /
        // is_sensitive flags for a position (database/schemas/auth.md).
        var accessTemplates = await _onboardingRepository.GetActivePositionAccessTemplatesAsync(tenantId, positionId.Value);
        foreach (var accessTemplate in accessTemplates)
        {
            if (accessTemplate.RequiresApproval || accessTemplate.IsSensitive)
            {
                return true;
            }
        }
        return false;
    }

    // ------------------------------------------------------------------
    // Blocked-draft follow-up actions
    // ------------------------------------------------------------------

    public async Task<DraftActionResponse> RequestSeatAsync(Guid id, CancellationToken ct)
    {
        var tenantId = GetCurrentTenantId();
        var userId = GetCurrentUserId();
        var draft = await _onboardingRepository.GetDraftByIdAsync(tenantId, id);

        if (draft == null)
        {
            return new DraftActionResponse { Status = "not_found" };
        }
        if (draft.Status != DraftStatusDraft || draft.DraftReason != ReasonWaitingForSeat)
        {
            return new DraftActionResponse { Status = "invalid_state", Message = "Seat increase can only be requested for a draft blocked with waiting_for_seat." };
        }

        // Billing Manager Inbox surrogate: a tenant-wide notification. There is no
        // dedicated seat-request table in this repo yet; the durable record is the
        // notification plus the SeatIncreaseRequested outbox event below.
        await _outboxRepository.AddNotificationAsync(new Notification
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            UserId = null,
            Title = "Seat increase requested",
            Body = $"Onboarding draft for {draft.EmployeeName} ({draft.WorkEmail}) is blocked because no employee seat is available.",
            IsRead = false,
            CreatedAtUtc = DateTime.UtcNow
        });

        await _outboxRepository.AddAsync(new OutboxMessage
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            Type = "SeatIncreaseRequested",
            PayloadJson = JsonSerializer.Serialize(new
            {
                draftId = draft.Id,
                workEmail = draft.WorkEmail,
                requestedByUserId = userId
            }),
            Status = "pending",
            RetryCount = 0,
            CreatedAtUtc = DateTime.UtcNow
        });

        await _onboardingRepository.SaveChangesAsync();
        return new DraftActionResponse { Status = "ok", Message = "Seat increase request sent to the billing manager inbox." };
    }

    public async Task<DraftActionResponse> SubmitPositionApprovalAsync(Guid id, CancellationToken ct)
    {
        var tenantId = GetCurrentTenantId();
        var userId = GetCurrentUserId();
        var draft = await _onboardingRepository.GetDraftByIdAsync(tenantId, id);

        if (draft == null)
        {
            return new DraftActionResponse { Status = "not_found" };
        }
        if (draft.Status != DraftStatusDraft || draft.DraftReason != ReasonWaitingForPositionApproval)
        {
            return new DraftActionResponse { Status = "invalid_state", Message = "Position approval can only be submitted for a draft blocked with waiting_for_position_approval." };
        }

        // The canonical approval table (access_grant_requests) requires an
        // employee_id and user_id, but during onboarding neither exists until the
        // final invite path runs. So the pending approval is persisted here as a
        // tenant-wide notification (Position Approver Inbox surrogate) plus a
        // durable PositionApprovalRequested outbox event. A pre-employee approval
        // record is a documented gap until the canonical model supports it.
        await _outboxRepository.AddNotificationAsync(new Notification
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            UserId = null,
            Title = "Position approval requested",
            Body = $"Onboarding draft for {draft.EmployeeName} ({draft.WorkEmail}) needs approval for a sensitive position.",
            IsRead = false,
            CreatedAtUtc = DateTime.UtcNow
        });

        await _outboxRepository.AddAsync(new OutboxMessage
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            Type = "PositionApprovalRequested",
            PayloadJson = JsonSerializer.Serialize(new
            {
                draftId = draft.Id,
                workEmail = draft.WorkEmail,
                positionId = draft.PositionId,
                departmentId = draft.DepartmentId,
                legalEntityId = draft.LegalEntityId,
                scheduleId = draft.ScheduleId,
                requestedByUserId = userId
            }),
            Status = "pending",
            RetryCount = 0,
            CreatedAtUtc = DateTime.UtcNow
        });

        await _onboardingRepository.SaveChangesAsync();
        return new DraftActionResponse { Status = "ok", Message = "Approval request sent to the position approver inbox." };
    }

    // ------------------------------------------------------------------
    // Finalize and send invite
    // ------------------------------------------------------------------

    public async Task<SendInviteResponse> SendInviteAsync(Guid id, CancellationToken ct)
    {
        var tenantId = GetCurrentTenantId();
        var userId = GetCurrentUserId();

        var draft = await _onboardingRepository.GetDraftByIdAsync(tenantId, id);
        if (draft == null)
        {
            return new SendInviteResponse { Status = "not_found", Error = "Draft not found." };
        }
        if (draft.Status != DraftStatusDraft)
        {
            return new SendInviteResponse { Status = "invalid", Error = "Draft is not in draft state." };
        }

        // Step 1: re-run pre-invite validation. Nothing is created when it fails.
        var validation = await RunPreInviteValidationAsync(draft, tenantId);
        if (!validation.IsValid)
        {
            return new SendInviteResponse { Status = "invalid", Validation = validation };
        }

        // Step 2: seat availability.
        var seat = await CheckSeatAvailabilityAsync(tenantId);
        if (seat.Warning != null)
        {
            validation.Warnings.Add(seat.Warning);
        }
        if (!seat.Available)
        {
            draft.DraftReason = ReasonWaitingForSeat;
            draft.UpdatedAtUtc = DateTime.UtcNow;
            await _onboardingRepository.SaveChangesAsync();

            return new SendInviteResponse
            {
                Status = "blocked",
                DraftReason = ReasonWaitingForSeat,
                Validation = validation,
                Actions = new List<string> { ActionRequestSeatIncrease }
            };
        }

        // Step 3: sensitive position approval.
        var approvalRequired = await IsPositionApprovalRequiredAsync(tenantId, draft.PositionId);
        if (approvalRequired)
        {
            draft.DraftReason = ReasonWaitingForPositionApproval;
            draft.UpdatedAtUtc = DateTime.UtcNow;
            await _onboardingRepository.SaveChangesAsync();

            return new SendInviteResponse
            {
                Status = "blocked",
                DraftReason = ReasonWaitingForPositionApproval,
                Validation = validation,
                Actions = new List<string> { ActionSubmitPositionApproval }
            };
        }

        // Step 4: clear to create. All rows below are committed in ONE
        // SaveChangesAsync at the end, so a failure leaves nothing behind.
        var rawToken = GenerateRawToken();
        var tokenHash = HashToken(rawToken);
        var (firstName, lastName) = SplitName(draft.EmployeeName);
        var hireDate = draft.StartDate ?? DateOnly.FromDateTime(DateTime.UtcNow);
        var employeeNumber = await ResolveEmployeeNumberAsync(tenantId, draft.EmployeeNumber);

        var newUser = new User
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            Email = draft.WorkEmail,
            DisplayName = draft.EmployeeName,
            PasswordHash = string.Empty,
            IsActive = false,
            PasswordSetupRequired = true,
            PasswordSetupExpiresAt = DateTimeOffset.UtcNow.AddHours(72),
            CreatedAtUtc = DateTime.UtcNow
        };
        await _userRepository.AddAsync(newUser);

        var newEmployee = new Employee
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            EmployeeNumber = employeeNumber,
            FirstName = firstName,
            LastName = lastName,
            WorkEmail = draft.WorkEmail,
            Status = "onboarding",
            HireDate = hireDate,
            // Validation guarantees LegalEntityId is present and tenant-owned.
            LegalEntityId = draft.LegalEntityId!.Value,
            DepartmentId = draft.DepartmentId,
            CurrentPositionId = draft.PositionId,
            CreatedAtUtc = DateTime.UtcNow
        };
        newUser.EmployeeId = newEmployee.Id;
        await _employeeRepository.AddAsync(newEmployee);

        if (draft.PositionId.HasValue)
        {
            var positionAssignment = new PositionAssignment
            {
                Id = Guid.NewGuid(),
                TenantId = tenantId,
                EmployeeId = newEmployee.Id,
                PositionId = draft.PositionId.Value,
                IsPrimary = true,
                StartsAtUtc = DateTime.UtcNow
            };
            await _orgRepository.AddPositionAssignmentAsync(positionAssignment);
        }

        var inviteToken = new InvitationToken
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            UserId = newUser.Id,
            InvitedEmail = newUser.Email,
            InvitedFullName = draft.EmployeeName,
            TokenHash = tokenHash,
            Status = "pending",
            CompletionMethodsJson = "[\"password\"]",
            AllowedEmailDomainsJson = "[]",
            ExpiresAt = DateTimeOffset.UtcNow.AddHours(72),
            CreatedAt = DateTimeOffset.UtcNow,
            CreatedByUserId = userId
        };
        await _userRepository.AddInvitationTokenAsync(inviteToken);

        var lifecycleEvent = new EmployeeLifecycleEvent
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            EmployeeId = newEmployee.Id,
            EventType = "hired",
            EventDate = hireDate,
            DetailsJson = "{\"source\":\"onboarding_invite\"}",
            PerformedById = userId,
            CreatedAt = DateTimeOffset.UtcNow
        };
        await _employeeRepository.AddLifecycleEventAsync(lifecycleEvent);

        await CreateChecklistTasksAsync(draft, tenantId, newEmployee, newUser);

        await _outboxRepository.AddNotificationAsync(new Notification
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            UserId = userId,
            Title = "Onboarding invite sent",
            Body = $"An invitation was sent to {draft.WorkEmail}.",
            IsRead = false,
            CreatedAtUtc = DateTime.UtcNow
        });

        // email_delivery_logs row is created with status=queued BEFORE any
        // dispatch. The rendered body snapshots are the ONLY persisted place
        // the raw invite token may appear — never invitation_tokens, never the
        // outbox payload. The processor endpoint sends queued rows through
        // SendGrid when the tenant has an active notification channel, or the
        // local_dev fallback when it does not.
        var inviteUrl = BuildInviteUrl(rawToken);
        var activeEmailChannel = await _notificationChannelRepository.GetActiveEmailChannelAsync(tenantId, ct);
        var emailLog = new EmailDeliveryLog
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            NotificationChannelId = activeEmailChannel?.Id,
            RecipientEmail = newUser.Email,
            SubjectSnapshot = "You're invited to OneVo",
            BodyHtmlSnapshot = BuildInviteEmailHtml(draft.EmployeeName, inviteUrl),
            BodyTextSnapshot = BuildInviteEmailText(draft.EmployeeName, inviteUrl),
            Provider = activeEmailChannel is null ? "local_dev" : "sendgrid",
            Status = "queued",
            AttemptCount = 0,
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.UtcNow
        };
        await _emailLogRepository.AddAsync(emailLog);

        // Durable EmployeeOnboardingStarted domain event. The payload never
        // contains the raw invite token.
        await _outboxRepository.AddAsync(new OutboxMessage
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            Type = "EmployeeOnboardingStarted",
            PayloadJson = JsonSerializer.Serialize(new
            {
                employeeId = newEmployee.Id,
                userId = newUser.Id,
                draftId = draft.Id,
                workEmail = draft.WorkEmail,
                legalEntityId = draft.LegalEntityId,
                departmentId = draft.DepartmentId,
                positionId = draft.PositionId
            }),
            Status = "pending",
            RetryCount = 0,
            CreatedAtUtc = DateTime.UtcNow
        });

        draft.Status = DraftStatusCompleted;
        draft.DraftReason = string.Empty;
        draft.UpdatedAtUtc = DateTime.UtcNow;

        // Single commit for user, employee, assignment, token, lifecycle event,
        // checklist tasks, notification, email log, outbox message, and draft
        // status. If this throws, the exception propagates and nothing persists.
        await _onboardingRepository.SaveChangesAsync();

        // The dev invite URL (containing the raw token) is returned only
        // outside Production so accept-invite can be tested without email.
        return new SendInviteResponse
        {
            Status = "completed",
            Validation = validation,
            EmployeeId = newEmployee.Id,
            UserId = newUser.Id,
            DevInviteUrl = _environment.IsProduction() ? null : inviteUrl
        };
    }

    private async Task CreateChecklistTasksAsync(OnboardingDraft draft, Guid tenantId, Employee newEmployee, User newUser)
    {
        var tasks = TryParseTasks(draft.EditedTasksJson);
        if (tasks == null)
        {
            // Unreachable in the finalize path: RunPreInviteValidationAsync has
            // already rejected invalid JSON before this method runs.
            throw new InvalidOperationException("Checklist tasks JSON is invalid.");
        }

        var sequence = 1;
        foreach (var task in tasks)
        {
            if (string.IsNullOrWhiteSpace(task.Title))
            {
                continue;
            }

            var checklistTask = new EmployeeChecklistTask
            {
                Id = Guid.NewGuid(),
                TenantId = tenantId,
                EmployeeId = newEmployee.Id,
                TemplateId = draft.SelectedTemplateId,
                LifecycleType = "onboarding",
                TaskTitle = task.Title,
                OwnerType = string.IsNullOrWhiteSpace(task.OwnerType) ? "employee" : task.OwnerType,
                Sequence = task.Sequence ?? sequence,
                // assigned_to_id is an FK to users; default to the new account.
                AssignedToId = task.AssignedToId ?? newUser.Id,
                DueDate = task.DueDate ?? draft.StartDate ?? DateOnly.FromDateTime(DateTime.UtcNow.AddDays(7)),
                Status = "pending"
                // is_required: the EmployeeChecklistTask model has no is_required
                // column (matching database/phase1-table-inventory.md), so the
                // required flag only lives in the template/edited tasks JSON.
            };
            await _employeeRepository.AddChecklistTaskAsync(checklistTask);
            sequence++;
        }
    }

    private async Task<string> ResolveEmployeeNumberAsync(Guid tenantId, string? requestedNumber)
    {
        if (!string.IsNullOrWhiteSpace(requestedNumber))
        {
            // Uniqueness of a supplied number was already checked by validation.
            return requestedNumber;
        }

        // Generate a unique number; retry a few times on the unlikely collision.
        for (var attempt = 0; attempt < 5; attempt++)
        {
            var candidate = "EMP-" + Guid.NewGuid().ToString("N").Substring(0, 8).ToUpperInvariant();
            var owner = await _employeeRepository.GetByEmployeeNumberAsync(tenantId, candidate);
            if (owner == null)
            {
                return candidate;
            }
        }
        throw new InvalidOperationException("Could not generate a unique employee number.");
    }

    // ------------------------------------------------------------------
    // Helpers
    // ------------------------------------------------------------------

    private sealed class DraftTaskDefinition
    {
        [JsonPropertyName("title")]
        public string? Title { get; set; }

        [JsonPropertyName("ownerType")]
        public string? OwnerType { get; set; }

        [JsonPropertyName("sequence")]
        public int? Sequence { get; set; }

        [JsonPropertyName("assignedToId")]
        public Guid? AssignedToId { get; set; }

        [JsonPropertyName("dueDate")]
        public DateOnly? DueDate { get; set; }

        [JsonPropertyName("isRequired")]
        public bool? IsRequired { get; set; }

        [JsonPropertyName("isLocked")]
        public bool? IsLocked { get; set; }
    }

    private static List<DraftTaskDefinition>? TryParseTasks(string? json)
    {
        if (string.IsNullOrWhiteSpace(json))
        {
            return new List<DraftTaskDefinition>();
        }

        try
        {
            var tasks = JsonSerializer.Deserialize<List<DraftTaskDefinition>>(json, TaskJsonOptions);
            return tasks ?? new List<DraftTaskDefinition>();
        }
        catch (JsonException)
        {
            return null;
        }
    }

    private static bool IsJsonArray(string json)
    {
        return TryParseTasks(json) != null;
    }

    private static ValidationIssue BuildIssue(string code, string message, string? field)
    {
        return new ValidationIssue { Code = code, Message = message, Field = field };
    }

    private Guid GetCurrentTenantId()
    {
        var tenantId = _currentUserService.TenantId;
        if (!tenantId.HasValue)
        {
            throw new UnauthorizedAccessException("No active tenant context.");
        }
        return tenantId.Value;
    }

    private Guid GetCurrentUserId()
    {
        var userId = _currentUserService.UserId;
        if (!userId.HasValue)
        {
            throw new UnauthorizedAccessException("No active user context.");
        }
        return userId.Value;
    }

    private string BuildInviteUrl(string rawToken)
    {
        var baseUrl = _emailOptions.FrontendBaseUrl.TrimEnd('/');
        return $"{baseUrl}/accept-invite?token={Uri.EscapeDataString(rawToken)}";
    }

    private static string BuildInviteEmailHtml(string employeeName, string inviteUrl)
    {
        var safeName = WebUtility.HtmlEncode(employeeName);
        var safeUrl = WebUtility.HtmlEncode(inviteUrl);

        return
            "<html><body style=\"font-family:Arial,sans-serif;color:#222;\">" +
            $"<p>Hi {safeName},</p>" +
            "<p>You've been invited to join OneVo. Click the button below to accept your invitation and set up your account. " +
            "This link expires in 72 hours.</p>" +
            $"<p><a href=\"{safeUrl}\" style=\"display:inline-block;padding:10px 20px;background:#4f46e5;color:#fff;text-decoration:none;border-radius:6px;\">Accept invitation</a></p>" +
            $"<p>If the button doesn't work, copy this link into your browser:<br/>{safeUrl}</p>" +
            "<p>— The OneVo team</p>" +
            "</body></html>";
    }

    private static string BuildInviteEmailText(string employeeName, string inviteUrl)
    {
        return
            $"Hi {employeeName},\n\n" +
            "You've been invited to join OneVo. Open the link below to accept your invitation " +
            "and set up your account. This link expires in 72 hours.\n\n" +
            $"{inviteUrl}\n\n" +
            "— The OneVo team";
    }

    private static string GenerateRawToken()
    {
        var bytes = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(bytes);
        return Convert.ToBase64String(bytes);
    }

    private static string HashToken(string rawToken)
    {
        var hashedBytes = SHA256.HashData(Encoding.UTF8.GetBytes(rawToken));
        return Convert.ToHexString(hashedBytes).ToLowerInvariant();
    }

    private static (string FirstName, string LastName) SplitName(string fullName)
    {
        if (string.IsNullOrWhiteSpace(fullName))
        {
            return (string.Empty, string.Empty);
        }
        var parts = fullName.Split(' ', 2, StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length == 1)
        {
            return (parts[0], string.Empty);
        }
        return (parts[0], parts[1]);
    }
}
