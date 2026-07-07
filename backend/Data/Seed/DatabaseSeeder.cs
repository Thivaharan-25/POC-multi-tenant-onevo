using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using OnevoHr.Api.Models.Auth;
using OnevoHr.Api.Models.Catalog;
using OnevoHr.Api.Models.DeveloperPlatform;
using OnevoHr.Api.Models.Employees;
using OnevoHr.Api.Models.Generated;
using OnevoHr.Api.Models.Leave;
using OnevoHr.Api.Models.Notifications;
using OnevoHr.Api.Models.OrgStructure;
using OnevoHr.Api.Models.Subscriptions;
using OnevoHr.Api.Models.Templates;
using OnevoHr.Api.Services.Interfaces;
using OnevoHr.Api.Repositories.Implementations;
using TenantEntity = OnevoHr.Api.Models.Tenant.Tenant;
using OnevoHr.Api.Models.Tenant;

namespace OnevoHr.Api.Data.Seed;

/// <summary>
/// Seeds the platform catalog, templates, and an active paid tenant (Acme HR).
/// Idempotent: runs only against an empty database.
/// </summary>
public static class DatabaseSeeder
{
    public static async Task SeedAsync(AppDbContext db, IPasswordHasher passwordHasher)
    {
        if (await db.PlatformUsers.AnyAsync())
        {
            return;
        }

        var now = DateTime.UtcNow;
        var passwordHash = passwordHasher.Hash("Password123!");

        // ------------------------------------------------------------------
        // 1-3. Platform permissions, roles, admin user
        // ------------------------------------------------------------------
        string[] platformPermissionKeys =
        {
            "platform.tenants.read", "platform.tenants.manage", "platform.tenants.activate",
            "platform.subscriptions.read", "platform.subscriptions.manage",
            "platform.module_catalog.read", "platform.module_catalog.manage",
            "platform.demo_profiles.read", "platform.demo_profiles.manage",
            "platform.requests.read", "platform.requests.manage",
            "platform.support.read", "platform.support.manage",
            "platform.accounts.read", "platform.accounts.manage",
            "platform.roles.read", "platform.roles.manage",
            "platform.templates.read", "platform.templates.manage",
            "platform.runtime_flags.read", "platform.runtime_flags.manage",
            "platform.security.read", "platform.security.manage",
            "platform.audit.read",
            "platform.reports.read",
            "platform.system_config.read", "platform.system_config.manage",
            "platform.operations.read", "platform.operations.manage",
            "platform.app_catalog.read", "platform.app_catalog.manage"
        };

        var platformPermissions = platformPermissionKeys.ToDictionary(
            key => key,
            key => new PlatformPermission
            {
                Id = Guid.NewGuid(),
                PermissionKey = key,
                Description = key.Replace("platform.", "").Replace('.', ' '),
                Category = key.Split('.')[1],
                // GetPermissionKeysForUserAsync filters on IsActive — inactive
                // seeded permissions would 403 every platform admin endpoint.
                IsActive = true,
            });
        db.PlatformPermissions.AddRange(platformPermissions.Values);

        var platformRoleDefinitions = new Dictionary<string, string[]>
        {
            ["Platform Super Admin"] = platformPermissionKeys,
            ["Tenant Operations Manager"] = new[]
            {
                "platform.tenants.read", "platform.tenants.manage", "platform.tenants.activate",
                "platform.requests.read", "platform.requests.manage",
                "platform.demo_profiles.read", "platform.templates.read", "platform.operations.read"
            },
            ["Billing Manager"] = new[]
            {
                "platform.subscriptions.read", "platform.subscriptions.manage", "platform.tenants.read"
            },
            ["Security Auditor"] = new[]
            {
                "platform.security.read", "platform.audit.read", "platform.tenants.read",
                "platform.accounts.read", "platform.roles.read"
            },
            ["Module Catalog Manager"] = new[]
            {
                "platform.module_catalog.read", "platform.module_catalog.manage",
                "platform.app_catalog.read", "platform.app_catalog.manage", "platform.templates.read"
            },
            ["Operations Engineer"] = new[]
            {
                "platform.operations.read", "platform.operations.manage",
                "platform.runtime_flags.read", "platform.runtime_flags.manage", "platform.system_config.read"
            },
            ["Read-Only Viewer"] = platformPermissionKeys.Where(k => k.EndsWith(".read")).ToArray()
        };

        var platformRoles = new Dictionary<string, PlatformRole>();
        foreach (var (roleName, permissionKeys) in platformRoleDefinitions)
        {
            var role = new PlatformRole
            {
                Id = Guid.NewGuid(),
                Name = roleName,
                Description = roleName,
                IsSystemRole = true,
                CreatedAtUtc = now
            };
            platformRoles[roleName] = role;
            db.PlatformRoles.Add(role);

            foreach (var key in permissionKeys)
            {
                db.PlatformRolePermissions.Add(new PlatformRolePermission
                {
                    Id = Guid.NewGuid(),
                    PlatformRoleId = role.Id,
                    PlatformPermissionId = platformPermissions[key].Id
                });
            }
        }

        var platformAdmin = new PlatformUser
        {
            Id = Guid.NewGuid(),
            Email = "platform.admin@onevo.test",
            PasswordHash = passwordHash,
            DisplayName = "Platform Admin",
            IsActive = true,
            MfaEnabled = false,
            CreatedAtUtc = now,
            UpdatedAtUtc = now
        };
        db.PlatformUsers.Add(platformAdmin);
        db.PlatformUserRoles.Add(new PlatformUserRole
        {
            Id = Guid.NewGuid(),
            PlatformUserId = platformAdmin.Id,
            PlatformRoleId = platformRoles["Platform Super Admin"].Id
        });

        // ------------------------------------------------------------------
        // 4-8. Tenant permission catalog, module catalog, features, ownership,
        //      feature-permission mapping
        // ------------------------------------------------------------------
        var moduleDefinitions = new (string Key, string Name, bool IsFoundation, bool IsSellable)[]
        {
            ("auth", "Auth & Security", true, false),
            ("configuration", "Configuration", true, false),
            ("roles", "Roles & Permissions", true, false),
            ("notifications", "Notifications", true, false),
            ("org", "Org Structure", true, false),
            ("core_hr", "Core HR", false, true),
            ("time_off", "Time Off", false, true),
            ("calendar", "Calendar", false, true),
            ("time_attendance", "Time & Attendance", false, true),
            ("monitoring", "Activity Monitoring", false, true),
            ("verification", "Identity Verification", false, true),
            ("analytics", "Analytics", false, true),
            ("work_management", "Work Management", false, true)
        };
        // Phase 2, not seeded: the "integrations" module (Microsoft Teams, GitHub,
        // webhooks, API access) — all of wms-integrations is excluded per
        // phase1-table-inventory.md and ADE-START-HERE.md.

        var modules = new Dictionary<string, ModuleCatalog>();
        foreach (var (key, name, isFoundation, isSellable) in moduleDefinitions)
        {
            var module = new ModuleCatalog
            {
                Id = Guid.NewGuid(),
                ModuleKey = key,
                DisplayName = name,
                Description = $"{name} module",
                IsFoundation = isFoundation,
                IsSellable = isSellable,
                IsActive = true,
                SupportsStorage = key is "employees" or "settings",
                SupportsAi = false,
                CreatedAtUtc = now
            };
            modules[key] = module;
            db.ModuleCatalogs.Add(module);
        }

        var featureDefinitions = new Dictionary<string, string[]>
        {
            ["auth"] = new[] { "auth.optional_google_oauth", "auth.mfa_enforcement" },
            ["configuration"] = new[] { "configuration.tenant_settings" },
            ["roles"] = new[] { "roles.permission_management" },
            ["notifications"] = new[] { "notifications.email_delivery", "notifications.in_app_delivery" },
            ["org"] = new[] { "org.structure_management" },
            ["core_hr"] = new[] { "core_hr.employee_profiles", "core_hr.employee_lifecycle", "core_hr.onboarding", "core_hr.offboarding", "core_hr.dependents_contacts", "core_hr.qualifications", "core_hr.compensation" },
            ["time_off"] = new[] { "time_off.requests", "time_off.approvals", "time_off.balances", "time_off.accrual_rules", "time_off.types", "time_off.calendar_integration" },
            ["calendar"] = new[] { "calendar.company_calendar", "calendar.holidays", "calendar.time_off_visibility", "calendar.event_sync" },
            ["time_attendance"] = new[] { "time_attendance.work_schedules" },
            ["monitoring"] = new[] { "monitoring.activity_tracking", "monitoring.app_usage", "monitoring.website_usage", "monitoring.idle_detection", "monitoring.screenshot_on_demand", "monitoring.app_allowlist", "monitoring.productivity_classification", "monitoring.raw_data_processing", "monitoring.presence_sessions", "monitoring.break_tracking", "monitoring.attendance_corrections", "monitoring.device_sessions", "monitoring.biometric_devices" },
            ["verification"] = new[] { "verification.identity_checks", "verification.face_match", "verification.verification_policies", "verification.manual_review", "verification.photo_challenge" },
            ["analytics"] = new[] { "analytics.daily_reports", "analytics.monthly_reports", "analytics.monitoring_snapshots", "analytics.productivity_dashboard", "analytics.data_export", "analytics.scheduled_reports" },
            // Phase 1 work management is projects, tasks, and simple time tracking only.
            // Sprints/boards/roadmaps (wms-planning), OKRs, resource planning,
            // work analytics, and GitHub integration are Phase 2.
            ["work_management"] = new[] { "work_management.projects", "work_management.tasks", "work_management.time_tracking" }
        };

        var features = new Dictionary<string, ModuleFeature>();
        foreach (var (moduleKey, featureKeys) in featureDefinitions)
        {
            foreach (var fk in featureKeys)
            {
                var feature = new ModuleFeature
                {
                    Id = Guid.NewGuid(),
                    ModuleCatalogId = modules[moduleKey].Id,
                    FeatureKey = fk,
                    IsActive = true
                };
                features[fk] = feature;
                db.ModuleFeatures.Add(feature);
            }
        }

        var permissionDefinitions = new (string PermissionKey, string ModuleKey, string FeatureKey)[]
        {
            ("org:legal-entities:read", "org", "org.structure_management"),
            ("org:legal-entities:manage", "org", "org.structure_management"),
            ("org:departments:read", "org", "org.structure_management"),
            ("org:departments:manage", "org", "org.structure_management"),
            ("org:positions:read", "org", "org.structure_management"),
            ("org:positions:manage", "org", "org.structure_management"),
            ("org:position-assignments:manage", "org", "org.structure_management"),
            ("org:position-roles:manage", "org", "org.structure_management"),
            ("attendance:read", "time_attendance", "time_attendance.work_schedules"),
            ("employees:read", "core_hr", "core_hr.employee_profiles"),
            ("employees:write", "core_hr", "core_hr.employee_profiles"),
            ("employees:read-own", "core_hr", "core_hr.employee_profiles"),
            ("employees:import", "core_hr", "core_hr.employee_profiles"),
            ("roles:read", "roles", "roles.permission_management"),
            ("roles:manage", "roles", "roles.permission_management"),
            ("permissions:read", "roles", "roles.permission_management"),
            ("permissions:manage", "roles", "roles.permission_management"),
            ("access:assign", "roles", "roles.permission_management"),
            ("leave:create", "time_off", "time_off.requests"),
            ("leave:read", "time_off", "time_off.requests"),
            ("leave:read-own", "time_off", "time_off.requests"),
            ("leave:approve", "time_off", "time_off.approvals"),
            ("leave:policies:read", "time_off", "time_off.types"),
            ("leave:policies:manage", "time_off", "time_off.types"),
            ("calendar:read", "calendar", "calendar.company_calendar"),
            ("calendar:manage", "calendar", "calendar.company_calendar"),
            ("calendar:holidays:manage", "calendar", "calendar.holidays"),
            ("settings:read", "configuration", "configuration.tenant_settings"),
            ("settings:manage", "configuration", "configuration.tenant_settings"),
            ("settings:security:read", "configuration", "configuration.tenant_settings"),
            ("settings:security:manage", "configuration", "configuration.tenant_settings"),
            ("settings:policies:read", "configuration", "configuration.tenant_settings"),
            ("settings:policies:manage", "configuration", "configuration.tenant_settings"),
            ("devices:read", "monitoring", "monitoring.device_sessions"),
            ("devices:manage", "monitoring", "monitoring.device_sessions"),
            ("billing:read", "configuration", "configuration.tenant_settings"),
            ("billing:manage", "configuration", "configuration.tenant_settings"),
            ("notifications:read", "notifications", "notifications.in_app_delivery"),
            ("notifications:manage", "notifications", "notifications.email_delivery"),
            ("approval:act", "time_off", "time_off.approvals")
        };

        var permissions = new Dictionary<string, OnevoHr.Api.Models.Generated.Permission>();
        foreach (var (permissionKey, moduleKey, fk) in permissionDefinitions)
        {
            var permission = new OnevoHr.Api.Models.Generated.Permission
            {
                Id = Guid.NewGuid(),
                Code = permissionKey,
                Description = $"{permissionKey} ({moduleKey})",
                Module = moduleKey,
                FeatureKey = fk
            };
            permissions[permissionKey] = permission;
            db.Permissions.Add(permission);

            db.ModulePermissionOwnerships.Add(new ModulePermissionOwnership
            {
                Id = Guid.NewGuid(),
                ModuleCatalogId = modules[moduleKey].Id,
                PermissionId = permission.Id
            });
        }

        // ------------------------------------------------------------------
        // 9-12. Subscription plans, plan features, price brackets (in-memory), add-ons
        // ------------------------------------------------------------------
        var growthPlan = new SubscriptionPlan
        {
            Id = Guid.NewGuid(),
            Name = "Growth",
            Code = "growth",
            BillingCycle = "monthly",
            IsActive = true,
            SharedBaseStorageGb = 100,
            SharedBaseAiTokenAllowance = 1_000_000,
            CreatedAtUtc = now
        };
        var enterprisePlan = new SubscriptionPlan
        {
            Id = Guid.NewGuid(),
            Name = "Enterprise",
            Code = "enterprise",
            BillingCycle = "annual",
            IsActive = true,
            SharedBaseStorageGb = 500,
            SharedBaseAiTokenAllowance = 5_000_000,
            CreatedAtUtc = now
        };
        db.SubscriptionPlans.AddRange(growthPlan, enterprisePlan);

        var planModules = new List<SubscriptionPlanModule>();
        foreach (var plan in new[] { growthPlan, enterprisePlan })
        {
            foreach (var module in modules.Values)
            {
                planModules.Add(new SubscriptionPlanModule
                {
                    Id = Guid.NewGuid(),
                    SubscriptionPlanId = plan.Id,
                    ModuleKey = module.ModuleKey,
                    PackageType = "base",
                    IsActive = true,
                    CreatedAtUtc = now
                });
            }
        }

        var priceBrackets = new (string Range, decimal Monthly, decimal Annual)[]
        {
            ("1-50", 199m, 1990m),
            ("51-200", 499m, 4990m),
            ("201-1000", 999m, 9990m),
            ("1001+", 1999m, 19990m)
        };
        var bracketEntities = new List<SubscriptionPlanPriceBracket>();
        foreach (var plan in new[] { growthPlan, enterprisePlan })
        {
            foreach (var (range, monthly, annual) in priceBrackets)
            {
                bracketEntities.Add(new SubscriptionPlanPriceBracket
                {
                    Id = Guid.NewGuid(),
                    SubscriptionPlanId = plan.Id,
                    CompanySizeRange = range,
                    BasePlanMonthlyPrice = monthly,
                    AnnualPrice = annual,
                    Currency = "USD",
                    CreatedAtUtc = now
                });
            }
        }

        var resourceAddOns = new List<SubscriptionPlanResourceAddon>();
        foreach (var plan in new[] { growthPlan, enterprisePlan })
        {
            resourceAddOns.Add(new SubscriptionPlanResourceAddon
            {
                Id = Guid.NewGuid(),
                SubscriptionPlanId = plan.Id,
                Label = "Extra Storage 50 GB",
                StorageContributionGb = 50,
                IsActive = true,
                CreatedAtUtc = now
            });
            resourceAddOns.Add(new SubscriptionPlanResourceAddon
            {
                Id = Guid.NewGuid(),
                SubscriptionPlanId = plan.Id,
                Label = "Extra AI Tokens 1M",
                AiTokenContribution = 1_000_000,
                IsActive = true,
                CreatedAtUtc = now
            });
        }

        SubscriptionRepository.SeedInMemoryPlanModules(planModules);
        SubscriptionRepository.SeedInMemoryBrackets(bracketEntities);
        SubscriptionRepository.SeedInMemoryResourceAddOns(resourceAddOns);

        // ------------------------------------------------------------------
        // 18-19. Configuration templates + role templates
        // ------------------------------------------------------------------
        var roleTemplateDefinitions = new (string Name, string[] ModuleKeys, string[] PermissionKeys)[]
        {
            ("HR Admin", new[] { "employees", "organization", "leave", "settings", "notifications", "time_attendance" }, new[]
            {
                "employees:read", "employees:write", "employees:import",
                "org:legal-entities:read", "org:departments:read", "org:departments:manage",
                "org:positions:read", "org:positions:manage",
                "org:position-assignments:manage", "org:position-roles:manage",
                "attendance:read",
                "roles:read", "permissions:read",
                "leave:policies:read", "leave:policies:manage",
                "settings:read", "settings:policies:manage",
                "notifications:read", "notifications:manage"
            }),
            ("Manager", new[] { "employees", "leave", "calendar" }, new[]
            {
                "employees:read", "org:positions:read", "leave:read", "leave:approve",
                "calendar:read", "approval:act", "notifications:read"
            }),
            ("Employee", new[] { "employees", "leave", "calendar" }, new[]
            {
                "employees:read-own", "leave:create", "leave:read-own", "calendar:read", "notifications:read"
            })
        };

        var roleTemplates = new Dictionary<string, RoleTemplate>();
        foreach (var (name, moduleKeys, permissionKeys) in roleTemplateDefinitions)
        {
            var resolvedPermissionKeys = permissionKeys.Where(k => permissions.ContainsKey(k)).ToArray();
            var roleTemplate = new RoleTemplate
            {
                Id = Guid.NewGuid(),
                Name = name,
                Description = $"{name} role template",
                ModuleKeysJson = JsonSerializer.Serialize(moduleKeys),
                PermissionCodesJson = JsonSerializer.Serialize(resolvedPermissionKeys),
                IsSystem = true,
                IsActive = true,
                CreatedAtUtc = now
            };
            roleTemplates[name] = roleTemplate;
            db.RoleTemplates.Add(roleTemplate);
        }

        var positionTemplateRanges = new (string Key, string Name, int Min, int? Max)[]
        {
            ("position-template-starter", "Starter Position Template", 1, 10),
            ("position-template-small-business", "Small Business Position Template", 11, 50),
            ("position-template-growth", "Growth Position Template", 51, 100),
            ("position-template-mid-market", "Mid-Market Position Template", 101, 500),
            ("position-template-enterprise", "Enterprise Position Template", 501, 1000),
            ("position-template-large-enterprise", "Large Enterprise Position Template", 1001, null)
        };

        const string positionTemplatePayload = """
        {
          "positions": [
            {
              "position_key": "ceo",
              "position_name": "CEO / Owner",
              "department_name": "Executive",
              "reports_to_position_key": null,
              "capacity": 1,
              "position_type": "unique",
              "linked_role_template_id": null
            },
            {
              "position_key": "hr-manager",
              "position_name": "HR Manager",
              "department_name": "People Operations",
              "reports_to_position_key": "ceo",
              "capacity": 1,
              "position_type": "unique",
              "linked_role_template_id": null
            },
            {
              "position_key": "engineering-manager",
              "position_name": "Engineering Manager",
              "department_name": "Engineering",
              "reports_to_position_key": "ceo",
              "capacity": 1,
              "position_type": "unique",
              "linked_role_template_id": null
            },
            {
              "position_key": "software-engineer",
              "position_name": "Software Engineer",
              "department_name": "Engineering",
              "reports_to_position_key": "engineering-manager",
              "capacity": 10,
              "position_type": "pooled",
              "linked_role_template_id": null
            }
          ]
        }
        """;

        foreach (var (key, name, min, max) in positionTemplateRanges)
        {
            db.ConfigurationTemplates.Add(new ConfigurationTemplate
            {
                Id = Guid.NewGuid(),
                TemplateKey = key,
                TemplateType = "position_template",
                Name = name,
                Description = $"{name} ({min}-{(max?.ToString() ?? "+")})",
                Version = 1,
                EmployeeRangeMin = min,
                EmployeeRangeMax = max,
                PayloadJson = positionTemplatePayload,
                IsSystem = true,
                IsActive = true,
                CreatedAtUtc = now
            });
        }

        const string leavePolicyPayload = """
        {
          "leave_types": [
            {
              "code": "annual",
              "name": "Annual Leave",
              "entitlement_days": 20,
              "requires_approval": true,
              "carry_forward_allowed": true,
              "carry_forward_limit": 5,
              "assignment_scope": "tenant",
              "department_template_keys": [],
              "position_template_keys": []
            },
            {
              "code": "sick",
              "name": "Sick Leave",
              "entitlement_days": 10,
              "requires_approval": false,
              "carry_forward_allowed": false,
              "carry_forward_limit": 0,
              "assignment_scope": "tenant",
              "department_template_keys": [],
              "position_template_keys": []
            }
          ]
        }
        """;

        db.ConfigurationTemplates.Add(new ConfigurationTemplate
        {
            Id = Guid.NewGuid(),
            TemplateKey = "leave-policy-standard",
            TemplateType = "leave_policy",
            Name = "Standard Leave Policy",
            Description = "Default annual and sick leave policy",
            Version = 1,
            PayloadJson = leavePolicyPayload,
            IsSystem = true,
            IsActive = true,
            CreatedAtUtc = now
        });

        db.ConfigurationTemplates.Add(new ConfigurationTemplate
        {
            Id = Guid.NewGuid(),
            TemplateKey = "data-import-mapping-standard",
            TemplateType = "data_import_mapping",
            Name = "Standard Data Import Mapping",
            Description = "Default employee CSV import mapping",
            Version = 1,
            PayloadJson = """{"mappings":[{"source":"employee_number","target":"EmployeeNumber"},{"source":"first_name","target":"FirstName"},{"source":"last_name","target":"LastName"},{"source":"work_email","target":"WorkEmail"},{"source":"hire_date","target":"HireDate"}]}""",
            IsSystem = true,
            IsActive = true,
            CreatedAtUtc = now
        });

        // ------------------------------------------------------------------
        // 20-25. Active tenant Acme HR + domain + subscription + limits +
        //        entitlements
        // ------------------------------------------------------------------
        var acme = new TenantEntity
        {
            Id = Guid.NewGuid(),
            Name = "Acme HR",
            Slug = "acme",
            Status = "active",
            Source = "operator_provisioning",
            ConfirmedEmployeeCount = 120,
            CreatedAtUtc = now.AddDays(-60),
            ActivatedAtUtc = now.AddDays(-30)
        };
        db.Tenants.Add(acme);

        var allFeatureKeys = features.Keys.OrderBy(k => k).ToArray();
        var acmeSubscription = new TenantSubscription
        {
            Id = Guid.NewGuid(),
            TenantId = acme.Id,
            SubscriptionPlanId = growthPlan.Id,
            Status = "active",
            BillingCycle = "monthly",
            ConfirmedEmployeeCount = 120,
            SelectedFeatureKeysJson = JsonSerializer.Serialize(allFeatureKeys),
            SelectedAddOnsJson = "[]",
            StartsAtUtc = acme.ActivatedAtUtc,
            CreatedAtUtc = acme.ActivatedAtUtc!.Value,
        };
        db.TenantSubscriptions.Add(acmeSubscription);

        db.SubscriptionInvoices.Add(new SubscriptionInvoice
        {
            Id = Guid.NewGuid(),
            TenantId = acme.Id,
            TenantSubscriptionId = acmeSubscription.Id,
            InvoiceNumber = "INV-2026-0001",
            Status = "paid",
            Amount = 499m,
            Currency = "USD",
            DueAtUtc = acme.ActivatedAtUtc,
            PaidAtUtc = acme.ActivatedAtUtc,
            CreatedAtUtc = acme.ActivatedAtUtc.Value
        });

        var acmeLimitList = new List<TenantResourceLimit>();
        acmeLimitList.Add(new TenantResourceLimit
        {
            Id = Guid.NewGuid(),
            TenantId = acme.Id,
            StorageLimitGb = growthPlan.SharedBaseStorageGb,
            AiTokenLimit = growthPlan.SharedBaseAiTokenAllowance,
            EmployeeLimit = null,
            Source = "paid_plan",
            CreatedAtUtc = acme.ActivatedAtUtc.Value
        });

        foreach (var module in modules.Values)
        {
            db.TenantModuleEntitlements.Add(new TenantModuleEntitlement
            {
                Id = Guid.NewGuid(),
                TenantId = acme.Id,
                ModuleCatalogId = module.Id,
                State = "subscription_included",
                IsEnabled = true
            });
        }

        SubscriptionRepository.SeedInMemoryLimits(acmeLimitList);

        var acmeFeatureEntList = new List<TenantFeatureEntitlement>();
        foreach (var feature in features.Values)
        {
            acmeFeatureEntList.Add(new TenantFeatureEntitlement
            {
                Id = Guid.NewGuid(),
                TenantId = acme.Id,
                ModuleFeatureId = feature.Id,
                IsEnabled = true
            });
        }

        db.TenantProvisioningStates.Add(new TenantProvisioningState
        {
            Id = Guid.NewGuid(),
            TenantId = acme.Id,
            CurrentStep = "completed",
            OrganizationInfoCompletedAtUtc = acme.CreatedAtUtc,
            AdminAccountCompletedAtUtc = acme.CreatedAtUtc,
            SubscriptionCompletedAtUtc = acme.ActivatedAtUtc,
            ModuleSelectionCompletedAtUtc = acme.ActivatedAtUtc,
            TemplateApplicationCompletedAtUtc = acme.ActivatedAtUtc,
            ActivationReady = true,
            LastUpdatedAtUtc = acme.ActivatedAtUtc.Value
        });

        db.TenantFeatureEntitlements.AddRange(acmeFeatureEntList);

        // ------------------------------------------------------------------
        // 26-27. Acme legal entity + departments
        // ------------------------------------------------------------------
        var acmeGlobal = new LegalEntity
        {
            Id = Guid.NewGuid(),
            TenantId = acme.Id,
            Name = "Acme Global",
            Code = "ACME-GLOBAL",
            Status = "active",
            OfficeAddressLabel = "Acme HQ Office",
            OfficeLatitude = 37.7749m,
            OfficeLongitude = -122.4194m,
            OfficeAllowedRadiusMeters = 100,
            CreatedAtUtc = acme.CreatedAtUtc
        };
        db.LegalEntities.Add(acmeGlobal);

        var departmentNames = new[] { "Executive", "People Operations", "Engineering", "Finance" };
        var departments = new Dictionary<string, Department>();
        foreach (var deptName in departmentNames)
        {
            var department = new Department
            {
                Id = Guid.NewGuid(),
                TenantId = acme.Id,
                LegalEntityId = acmeGlobal.Id,
                Name = deptName,
                Code = deptName.ToUpperInvariant().Replace(' ', '_'),
                Status = "active",
                CreatedAtUtc = acme.CreatedAtUtc
            };
            departments[deptName] = department;
            db.Departments.Add(department);
        }

        // ------------------------------------------------------------------
        // 28-29. Acme tenant roles + role permissions
        // ------------------------------------------------------------------
        var tenantRoleDefinitions = new Dictionary<string, string[]>
        {
            ["Tenant Owner"] = permissions.Keys.ToArray(),
            ["HR Admin"] = roleTemplateDefinitions[0].PermissionKeys,
            ["Manager"] = roleTemplateDefinitions[1].PermissionKeys,
            ["Employee"] = roleTemplateDefinitions[2].PermissionKeys
        };

        var acmeRoles = new Dictionary<string, Role>();
        foreach (var (roleName, permissionKeys) in tenantRoleDefinitions)
        {
            var role = new Role
            {
                Id = Guid.NewGuid(),
                TenantId = acme.Id,
                Name = roleName,
                Description = roleName,
                IsSystemRole = true,
                SourceTemplateId = roleTemplates.TryGetValue(roleName, out var template) ? template.Id : null,
                CreatedAtUtc = acme.CreatedAtUtc
            };
            acmeRoles[roleName] = role;
            db.Roles.Add(role);

            foreach (var key in permissionKeys)
            {
                if (permissions.TryGetValue(key, out var perm))
                {
                    db.RolePermissions.Add(new RolePermission
                    {
                        Id = Guid.NewGuid(),
                        RoleId = role.Id,
                        PermissionId = perm.Id
                    });
                }
            }
        }

        // ------------------------------------------------------------------
        // 30-31. Acme positions + reporting history
        // ------------------------------------------------------------------
        Position MakePosition(string posName, string code, string departmentName, Guid? reportsTo, string roleName) => new()
        {
            Id = Guid.NewGuid(),
            TenantId = acme.Id,
            LegalEntityId = acmeGlobal.Id,
            DepartmentId = departments[departmentName].Id,
            Name = posName,
            Code = code,
            ReportsToPositionId = reportsTo,
            Capacity = 1,
            PositionType = "unique",
            Status = "active",
            SuggestedRoleId = acmeRoles[roleName].Id,
            CreatedAtUtc = acme.CreatedAtUtc
        };

        var ceoPosition = MakePosition("CEO / Owner", "CEO", "Executive", null, "Tenant Owner");
        var hrManagerPosition = MakePosition("HR Manager", "HR-MGR", "People Operations", ceoPosition.Id, "HR Admin");
        var engManagerPosition = MakePosition("Engineering Manager", "ENG-MGR", "Engineering", ceoPosition.Id, "Manager");
        var engineerPosition = MakePosition("Software Engineer", "SWE", "Engineering", engManagerPosition.Id, "Employee");
        db.Positions.AddRange(ceoPosition, hrManagerPosition, engManagerPosition, engineerPosition);

        // Default work schedule so onboarding's schedule dropdown has a real option.
        db.WorkSchedules.Add(new WorkSchedule
        {
            Id = Guid.NewGuid(),
            TenantId = acme.Id,
            LegalEntityId = acmeGlobal.Id,
            Name = "Standard Weekday (Mon–Fri)",
            Timezone = "UTC",
            DefaultForNewEmployee = true,
            IsActive = true,
            CreatedAt = acme.CreatedAtUtc,
            UpdatedAt = acme.CreatedAtUtc
        });

        foreach (var position in new[] { hrManagerPosition, engManagerPosition, engineerPosition })
        {
            db.PositionReportingHistories.Add(new PositionReportingHistory
            {
                Id = Guid.NewGuid(),
                TenantId = acme.Id,
                PositionId = position.Id,
                ReportsToPositionId = position.ReportsToPositionId,
                EffectiveFromUtc = acme.CreatedAtUtc
            });
        }

        // ------------------------------------------------------------------
        // 32-36. Acme employees, users, user roles, position assignments
        // ------------------------------------------------------------------
        Employee MakeEmployee(string number, string firstName, string lastName, string email, string departmentName, Guid positionId) => new()
        {
            Id = Guid.NewGuid(),
            TenantId = acme.Id,
            EmployeeNumber = number,
            FirstName = firstName,
            LastName = lastName,
            WorkEmail = email,
            Status = "active",
            HireDate = new DateOnly(2024, 1, 15),
            LegalEntityId = acmeGlobal.Id,
            DepartmentId = departments[departmentName].Id,
            CurrentPositionId = positionId,
            CreatedAtUtc = acme.CreatedAtUtc
        };

        var priya = MakeEmployee("EMP-001", "Priya", "Sharma", "owner@acme.test", "Executive", ceoPosition.Id);
        var maria = MakeEmployee("EMP-002", "Maria", "Gomez", "hr.admin@acme.test", "People Operations", hrManagerPosition.Id);
        var daniel = MakeEmployee("EMP-003", "Daniel", "Chen", "manager@acme.test", "Engineering", engManagerPosition.Id);
        var asha = MakeEmployee("EMP-004", "Asha", "Perera", "employee@acme.test", "Engineering", engineerPosition.Id);
        db.Employees.AddRange(priya, maria, daniel, asha);

        User MakeUser(string email, string displayName, Guid employeeId) => new()
        {
            Id = Guid.NewGuid(),
            TenantId = acme.Id,
            Email = email,
            PasswordHash = passwordHash,
            DisplayName = displayName,
            EmployeeId = employeeId,
            IsActive = true,
            CreatedAtUtc = acme.CreatedAtUtc
        };

        var ownerUser = MakeUser("owner@acme.test", "Priya Sharma", priya.Id);
        var hrAdminUser = MakeUser("hr.admin@acme.test", "Maria Gomez", maria.Id);
        var managerUser = MakeUser("manager@acme.test", "Daniel Chen", daniel.Id);
        var employeeUser = MakeUser("employee@acme.test", "Asha Perera", asha.Id);
        db.Users.AddRange(ownerUser, hrAdminUser, managerUser, employeeUser);

        var userRoleAssignments = new (User User, string RoleName, string? ScopeType)[]
        {
            (ownerUser, "Tenant Owner", "tenant"),
            (hrAdminUser, "HR Admin", null),
            (managerUser, "Manager", null),
            (employeeUser, "Employee", null)
        };
        foreach (var (user, roleName, scopeType) in userRoleAssignments)
        {
            db.UserRoles.Add(new UserRole
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                RoleId = acmeRoles[roleName].Id,
                ScopeType = scopeType,
                CreatedAtUtc = acme.CreatedAtUtc
            });
        }

        var positionAssignments = new (Employee Employee, Position Position)[]
        {
            (priya, ceoPosition),
            (maria, hrManagerPosition),
            (daniel, engManagerPosition),
            (asha, engineerPosition)
        };
        foreach (var (employee, position) in positionAssignments)
        {
            db.PositionAssignments.Add(new PositionAssignment
            {
                Id = Guid.NewGuid(),
                TenantId = acme.Id,
                PositionId = position.Id,
                EmployeeId = employee.Id,
                StartsAtUtc = acme.CreatedAtUtc,
                IsPrimary = true
            });
        }

        // Hierarchy closure: self rows (depth 0) plus manager chains.
        var closurePairs = new (Guid Manager, Guid Report, int Depth)[]
        {
            (priya.Id, priya.Id, 0),
            (maria.Id, maria.Id, 0),
            (daniel.Id, daniel.Id, 0),
            (asha.Id, asha.Id, 0),
            (priya.Id, maria.Id, 1),
            (priya.Id, daniel.Id, 1),
            (priya.Id, asha.Id, 2),
            (daniel.Id, asha.Id, 1)
        };
        foreach (var (manager, report, depth) in closurePairs)
        {
            db.EmployeeHierarchyClosures.Add(new EmployeeHierarchyClosure
            {
                Id = Guid.NewGuid(),
                TenantId = acme.Id,
                ManagerEmployeeId = manager,
                ReportEmployeeId = report,
                Depth = depth
            });
        }

        // Basic leave setup so the leave endpoints have data.
        var annualLeave = new LeaveType
        {
            Id = Guid.NewGuid(),
            TenantId = acme.Id,
            Code = "annual",
            Name = "Annual Leave",
            IsPaid = true,
            RequiresApproval = true
        };
        db.LeaveTypes.Add(annualLeave);

        var annualPolicy = new LeavePolicy
        {
            Id = Guid.NewGuid(),
            TenantId = acme.Id,
            LeaveTypeId = annualLeave.Id,
            EntitlementDays = 20,
            CarryForwardAllowed = true,
            CarryForwardLimit = 5,
            RequiresApproval = true,
            CreatedAtUtc = acme.CreatedAtUtc
        };
        db.LeavePolicies.Add(annualPolicy);
        db.LeavePolicyAssignments.Add(new LeavePolicyAssignment
        {
            Id = Guid.NewGuid(),
            TenantId = acme.Id,
            LeavePolicyId = annualPolicy.Id,
            AssignmentScope = "tenant"
        });

        foreach (var employee in new[] { priya, maria, daniel, asha })
        {
            db.LeaveBalances.Add(new LeaveBalance
            {
                Id = Guid.NewGuid(),
                TenantId = acme.Id,
                EmployeeId = employee.Id,
                LeaveTypeId = annualLeave.Id,
                EntitledDays = 20,
                UsedDays = 0,
                RemainingDays = 20,
                Year = now.Year
            });
        }

        // ------------------------------------------------------------------
        // 40. Dormant payment gateway configs (Stripe)
        // ------------------------------------------------------------------
        var stripeConfig = new PaymentGatewayConfig
        {
            Id = Guid.NewGuid(),
            GatewayKey = "stripe-global-sandbox",
            Provider = "stripe",
            Environment = "sandbox",
            DisplayName = "Stripe Gateway Sandbox",
            LogoUrl = "https://brand.stripe.com/stripe-logo.png",
            PublicKey = "pk_test_mock_key",
            MerchantId = null,
            WebhookUrl = "http://localhost:5255/api/v1/billing/webhooks/stripe",
            IsActive = false, // dormant
            CreatedById = platformAdmin.Id,
            CreatedAtUtc = now
        };
        db.PaymentGatewayConfigs.Add(stripeConfig);

        db.PaymentGatewayCredentials.Add(new PaymentGatewayCredential
        {
            Id = Guid.NewGuid(),
            PaymentGatewayConfigId = stripeConfig.Id,
            SecretEncrypted = System.Text.Encoding.UTF8.GetBytes("sk_test_mock_secret"),
            WebhookSecretEncrypted = System.Text.Encoding.UTF8.GetBytes("whsec_mock_secret"),
            EncryptionKeyVersion = "v1",
            CredentialVersion = 1,
            IsActive = true,
            RotatedById = platformAdmin.Id,
            RotatedAtUtc = now
        });

        db.PaymentGatewayCountryRoutes.Add(new PaymentGatewayCountryRoute
        {
            Id = Guid.NewGuid(),
            CountryCode = "US",
            CountryNameSnapshot = "United States",
            GatewayConfigId = stripeConfig.Id,
            Environment = "sandbox",
            IsActive = true,
            CreatedById = platformAdmin.Id,
            CreatedAtUtc = now
        });

        await db.SaveChangesAsync();
    }
}
