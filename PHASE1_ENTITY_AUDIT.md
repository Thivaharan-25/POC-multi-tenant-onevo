# OneVo-HR Phase 1 Entity Audit

This audit documents the alignment of database models and seed data against the canonical Phase 1 planning guidelines.

| Entity Name | Exists in Backend | Backend File Path | DbSet Present? | Seeded? | Missing Fields / Wrong Relationships | Decision | Reason |
| :--- | :---: | :--- | :---: | :---: | :--- | :---: | :--- |
| **Tenant** | Yes | `backend/Models/Tenant/Tenant.cs` | Yes | Yes | None | Keep | Matches Multi-tenant needs. |
| **LegalEntity** (Company) | Yes | `backend/Models/OrgStructure/LegalEntity.cs` | Yes | Yes | Missing 4 office columns: `OfficeAddressLabel`, `OfficeLatitude`, `OfficeLongitude`, `OfficeAllowedRadiusMeters`. | Fix | Office details reside directly on legal entities; branches map to separate entities. |
| **Work Location** (Office) | No | N/A | No | N/A | Stored on `legal_entities` directly. No separate table required in Phase 1. | Keep | Replaced by single office fields inside `legal_entities` according to docs. |
| **Department** | Yes | `backend/Models/OrgStructure/Department.cs` | Yes | Yes | None | Keep | Maps groups of seats. |
| **Position** | Yes | `backend/Models/OrgStructure/Position.cs` | Yes | Yes | None | Keep | Defines seats for reporting hierarchy. |
| **Employee** | Yes | `backend/Models/Employees/Employee.cs` | Yes | Yes | None | Keep | Master worker profile record. |
| **User** | Yes | `backend/Models/Auth/User.cs` | Yes | Yes | None | Keep | Maps credentials to worker profiles. |
| **PositionAssignment** | Yes | `backend/Models/OrgStructure/PositionAssignment.cs` | Yes | Yes | None | Keep | Links worker profiles to seat assignments. |
| **EmployeeHierarchyClosure** | Yes | `backend/Models/OrgStructure/EmployeeHierarchyClosure.cs` | Yes | Yes | None | Keep | Derived reporting tree cache. |
| **PermissionCatalog** | Yes | `backend/Models/Catalog/PermissionCatalog.cs` | Yes | Yes | None | Keep | Holds global permission codes catalog. |
| **TenantFeatureEntitlement** | Yes | `backend/Models/Subscriptions/TenantFeatureEntitlement.cs` | Yes | Yes | None | Keep | Commercial features allowed. |
| **TenantResourceLimit** | Yes | `backend/Models/Subscriptions/TenantResourceLimit.cs` | Yes | Yes | None | Keep | Storage and token quotas. |
| **ExternalCalendarConnection** | No | `backend/Models/Calendar/ExternalCalendarConnection.cs` | No | No | Entity does not exist. | Fix | User-level OAuth calendar config is required by docs. |
| **ExternalCalendarEventLink** | No | `backend/Models/Calendar/ExternalCalendarEventLink.cs` | No | No | Entity does not exist. | Fix | Sync state link matching Google/Outlook events is required by docs. |
| **PaymentGatewayConfig** | No | `backend/Models/Subscriptions/PaymentGatewayConfig.cs` | No | No | Entity does not exist. | Fix | Stripe gateway config records are required by docs. |
| **PaymentGatewayCredential** | No | `backend/Models/Subscriptions/PaymentGatewayCredential.cs` | No | No | Entity does not exist. | Fix | Stores encrypted Stripe client secrets. |
| **PaymentGatewayCountryRoute** | No | `backend/Models/Subscriptions/PaymentGatewayCountryRoute.cs` | No | No | Entity does not exist. | Fix | Routes local country requests to configs. |
| **OutboxMessage** | Yes | `backend/Models/Notifications/OutboxMessage.cs` | Yes | Yes | None | Keep | Core event-outbox infrastructure. |
