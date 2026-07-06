# Developer Platform Seed Audit

This audit validates the platform and developer seed dataset required for local tenant development.

## Seed Categories

| Seed Category | Required by Docs? | Source Doc Path | Existing Seed? | Existing Seed File Path | Missing Data | Decision | Reason |
| :--- | :---: | :--- | :---: | :--- | :--- | :---: | :--- |
| Platform Users | yes | developer-platform/database/schema.md | yes | backend/Data/Seed/DatabaseSeeder.cs | None | keep | platform.admin@onevo.test seeded correctly. |
| Platform Roles | yes | developer-platform/database/schema.md | yes | backend/Data/Seed/DatabaseSeeder.cs | None | keep | Platform Super Admin, billing/security roles seeded. |
| Platform Permission Keys | yes | developer-platform/database/schema.md | yes | backend/Data/Seed/DatabaseSeeder.cs | None | keep | Platform-wide read/write keys seeded. |
| Platform Sessions/Auth Events | yes | developer-platform/database/schema.md | no | None | No seed rows generated for platform logs | defer | Dormant infrastructure tables. |
| Module Catalog Rows | yes | backend/module-catalog.md | yes | backend/Data/Seed/DatabaseSeeder.cs | None | keep | All core and sellable modules seeded. |
| Module Feature Rows | yes | backend/module-catalog.md | yes | backend/Data/Seed/DatabaseSeeder.cs | None | keep | Detailed sub-module features seeded. |
| Feature Keys | yes | backend/module-catalog.md | yes | backend/Data/Seed/DatabaseSeeder.cs | None | keep | Enabled feature mapping keys seeded. |
| Permission Catalog Rows | yes | phase-1-feature-permission-map.md | yes | backend/Data/Seed/DatabaseSeeder.cs | None | keep | Total of 40+ HRMS permission catalog rows seeded. |
| Module Permission Ownership | yes | phase-1-feature-permission-map.md | yes | backend/Data/Seed/DatabaseSeeder.cs | None | keep | Correct mapping links seeded. |
| Subscription Plans | yes | phase1-table-inventory.md | yes | backend/Data/Seed/DatabaseSeeder.cs | None | keep | Growth and Enterprise plans seeded. |
| Base Packages | yes | phase1-table-inventory.md (`subscription_plan_modules`) | yes | backend/Data/Seed/DatabaseSeeder.cs -> SubscriptionRepository.SeedInMemoryPlanModules | None | keep | All Phase 1 modules seeded with `package_type = base` for Growth and Enterprise plans. No automation add-on is seeded (Workflow/Automation Engine is Phase 2). |
| Resource Add-Ons | yes | phase1-table-inventory.md (`subscription_plan_resource_addons`) | yes | backend/Data/Seed/DatabaseSeeder.cs -> SubscriptionRepository.SeedInMemoryResourceAddOns | None | keep | Extra storage (50GB) and extra AI token (1M) resource add-ons seeded per plan. |
| Tenant Plan Assignment | yes | phase1-table-inventory.md | yes | backend/Data/Seed/DatabaseSeeder.cs | None | keep | Linkage active for acme and beta tenants. |
| Tenant Module Entitlements | yes | phase1-table-inventory.md | yes | backend/Data/Seed/DatabaseSeeder.cs | None | keep | Assigned modules enabled for ACME. |
| Tenant Feature Entitlements | no (not in phase1-table-inventory.md's 199 headings) | developer-platform/database/schema.md | yes, in-memory only | backend/Data/Seed/DatabaseSeeder.cs -> SubscriptionRepository.SeedInMemoryFeatureEntitlements | Not persisted via AppDbContext/EF at all | keep as in-memory | Assigned features enabled for ACME via a static in-memory store, not an EF-mapped table; not a Phase 1 canonical table per phase1-table-inventory.md. |
| Tenant Resource Limits | no (not in phase1-table-inventory.md's 199 headings) | developer-platform/database/schema.md | yes, in-memory only | backend/Data/Seed/DatabaseSeeder.cs -> SubscriptionRepository.SeedInMemoryLimits | Not persisted via AppDbContext/EF at all | keep as in-memory | AI token and Storage GB caps seeded via a static in-memory store, not an EF-mapped table; not a Phase 1 canonical table per phase1-table-inventory.md. |
| Dormant Stripe config rows | yes | phase1-table-inventory.md | no | None | Dormant config configurations and credentials. | add seed | Seeding dormant Stripe gateway configuration for ACME billing testing. |
| Dormant Google/Outlook connections | yes | calendar.md | no | None | Dormant calendar sync connection config. | defer | OAuth connection tables are seeded on demand by test cases or API, not as base tenant database configuration. |
| ACME tenant seeding mapping | yes | ADE-START-HERE.md | yes | backend/Data/Seed/DatabaseSeeder.cs | Location mapping fields (office_*) in database seeder. | add seed | Update Acme Global seed with canonical office parameters. |
