# Phase 1 Event / Audit / Notification / Email / Webhook Trigger Map

**Status:** Documentation only. No code created in this step.
**Scope:** `C:\onevoNew\test` simplified test backend, modeled on the OneVo-HR Phase 1 specs.

---

## 1. Source Docs Read

- `C:\onevoNew\OneVo-HR\backend\domain-events.md`
- `C:\onevoNew\OneVo-HR\backend\folder-structure.md`
- `C:\onevoNew\OneVo-HR\backend\notification-system.md`
- `C:\onevoNew\OneVo-HR\database\phase1-table-inventory.md`
- `C:\onevoNew\OneVo-HR\ADE-START-HERE.md`

Relevant rules extracted:

- **Domain events are optional.** Use them only when a completed use case needs decoupled post-save side effects. Most commands should not raise a domain event. The default flow is `Controller -> Command/Query -> Validator -> Handler -> Repository/Domain -> UnitOfWork -> Response`; the event branch (`UnitOfWork save succeeds -> DomainEventDispatchInterceptor -> EventHandler(s)`) is added only by exception.
- **No external event bus in Phase 1.** `IEventBus`, `IntegrationEvent`, MassTransit, and RabbitMQ are explicitly out of scope. Domain events are in-process MediatR notifications only, dispatched after `SaveChangesAsync` succeeds.
- **No outbox table for external async publishing in Phase 1.** Outbox tables exist only to support external async message publishing, which Phase 1 does not have.
- **No Workflow/Automation Engine.** Phase 1 approvals (time off, transfers, access grants) are lightweight module-owned approval records (e.g. `access_grant_requests`, `employee_transfers`), not Workflow Engine instances. Workflow/Automation Engine and Exception Engine's full rule engine are Phase 2.
- **Notifications are Phase 1** through the 6-step pipeline (Preferences Check -> Deduplication -> Template Rendering -> Channel Routing -> Dispatch -> Track) writing to `notifications` (in-app) and `email_delivery_logs` (Resend email attempts). Chat and Inbox action cards: Inbox action cards are Phase 1, Chat action cards are Phase 2. Monitoring/verification alert routing goes through direct module-owned recipient resolution (management coverage chain or reporting manager), not a configurable rule engine.

---

## 2. Event Placement Decision

This test backend is a simplified mirror of the OneVo-HR Clean Architecture layering, collapsed to a single `backend/` folder instead of `ONEVO.Domain` / `ONEVO.Application` / `ONEVO.Infrastructure` projects. To keep the same "optional, by-exception" placement rule, future event classes and handlers go here:

**Domain event records** (only when a slice is justified):
```
C:\onevoNew\test\backend\Models\{Feature}\Events\
```
Examples:
```
C:\onevoNew\test\backend\Models\Employees\Events\EmployeeTerminatedEvent.cs
C:\onevoNew\test\backend\Models\Leave\Events\TimeOffRequestedEvent.cs
C:\onevoNew\test\backend\Models\Monitoring\Events\AppAllowlistViolationDetectedEvent.cs
```

**Event handlers** (only when a slice is justified):
```
C:\onevoNew\test\backend\Services\EventHandlers\{Feature}\
```
Examples:
```
C:\onevoNew\test\backend\Services\EventHandlers\Employees\RevokeAccessOnEmployeeTerminatedHandler.cs
C:\onevoNew\test\backend\Services\EventHandlers\Leave\NotifyApproverOnTimeOffRequestedHandler.cs
```

Neither folder is created by this step. They are created only when a specific command's implementation slice needs a decoupled post-save reaction, matching the OneVo-HR rule that `Events/` and `EventHandlers/` are not part of the default feature skeleton.

Audit log writes, notification writes, and email delivery log writes are **not** routed through domain events by default — they are written directly inside the command handler (or a shared audit/notification service called from the handler) unless a specific case genuinely needs decoupling (see Section 3). This mirrors OneVo-HR: most commands should not raise an event just to write an audit row.

---

## 3. Trigger Classification Rules

**Audit log** — Use for security, tenant configuration, permission, billing, employee lifecycle, monitoring policy, and admin-sensitive changes. Written synchronously in the same command/handler that performs the change (or a directly-called shared audit service), not via a domain event, unless the action already raises an event for another reason.

**Notification** — Use when a user must know or act (approval needed, request decided, alert requires acknowledgement, account/security event needs visibility).

**Email delivery log** — Use when a notification must leave the app by email (per `notification_templates` / `notification_preferences` channel routing). Every Resend-backed send creates an `email_delivery_logs` row before dispatch, regardless of whether a domain event is involved.

**Webhook delivery** — Use only for tenant-configured outbound webhook events (rows in `webhook_endpoints` subscribed via `events` jsonb), never for internal workflow or internal side effects. If no tenant has registered a `webhook_endpoints` row for an event type, no delivery is attempted.

**In-process domain event** — Use only when post-save side effects should be decoupled from the original command: the command has already completed its own responsibility, one or more secondary reactions should follow, the command shouldn't need to know every downstream reaction, and the reaction is independently testable. In this Phase 1 map, only `EmployeeTerminatedEvent` is scoped as a real slice; every other row marks domain event = No and describes the trigger as something the command/service does directly.

**No event** — Use when behavior is simple CRUD or the logic belongs directly in the command/service (e.g., plain read-tracking, straightforward create/update with no audit/notify/email/webhook need).

---

## 4. Phase 1 Trigger Matrix

| Module | Business action | Trigger/event name | Domain event? | Audit log? | Notification? | Email? | Webhook? | Tables touched | Reason | Priority | Notes |
|---|---|---|---|---|---|---|---|---|---|---|---|
| Auth & Security | User login succeeded | `auth.login_succeeded` | No | Yes | No | No | No | `sessions`, `audit_logs`, `users` | Security-sensitive access record | P1 | Update `users.last_login_at` in same handler |
| Auth & Security | User login failed | `auth.login_failed` | No | Yes | No | No | No | `audit_logs` | Security event, brute-force forensics | P1 | No notification unless lockout threshold added later |
| Auth & Security | Logout / session revoked | `auth.session_revoked` | No | Yes | No | No | No | `sessions`, `audit_logs` | Security-sensitive | P2 | Admin-initiated revoke also notifies affected user (see below) |
| Auth & Security | Session revoked by admin | `auth.session_revoked_by_admin` | No | Yes | Yes | No | No | `sessions`, `audit_logs`, `notifications` | Admin action + user must know | P2 | |
| Auth & Security | Password reset requested | `auth.password_reset_requested` | No | Yes | No | Yes | No | `users`, `audit_logs`, `email_delivery_logs` | Security-sensitive; user must receive reset link | P1 | Email is the delivery mechanism, not a separate in-app notification |
| Auth & Security | Password reset completed | `auth.password_reset_completed` | No | Yes | Yes | No | No | `users`, `audit_logs`, `notifications` | Security-sensitive; confirm to user | P1 | In-app confirmation only, no email required |
| Auth & Security | MFA enabled | `auth.mfa_enabled` | No | Yes | Yes | No | No | `user_mfa`, `audit_logs`, `notifications` | Security-sensitive; user should be informed | P2 | |
| Auth & Security | MFA failed | `auth.mfa_failed` | No | Yes | No | No | No | `audit_logs` | Security event | P2 | Repeated failures are a forensic signal, not a user-facing notification |
| Auth & Security | Role assigned | `auth.role_assigned` | No | Yes | Yes | No | No | `user_roles`, `audit_logs`, `notifications` | Permission change (admin-sensitive) + user should know | P1 | Category `system` per notification-system.md |
| Auth & Security | Role permission changed | `auth.role_permission_changed` | No | Yes | No | No | No | `role_permissions`, `audit_logs` | Permission change, admin-sensitive | P1 | Broad blast radius; no per-user notification, audit only |
| Auth & Security | User permission override granted/revoked | `auth.permission_override_changed` | No | Yes | Yes | No | No | `user_permission_overrides`, `audit_logs`, `notifications` | Permission change + affected user should know | P2 | |
| Auth & Security | Access grant request submitted | `auth.access_grant_requested` | No | Yes | Yes | No | No | `access_grant_requests`, `audit_logs`, `notifications` | Lightweight Phase 1 approval record; approver must act | P1 | Not a Workflow Engine instance |
| Auth & Security | Access grant request approved/rejected | `auth.access_grant_decided` | No | Yes | Yes | No | No | `access_grant_requests`, `user_roles`, `audit_logs`, `notifications` | Permission change + requester must know | P1 | Approval creates `user_roles` row when approved |
| Core HR | Employee created/onboarded | `employee.onboarded` | No | Yes | Yes | Yes | No | `employees`, `users`, `invitation_tokens`, `employee_lifecycle_events`, `audit_logs`, `notifications`, `email_delivery_logs` | Employee lifecycle (audit) + HR/manager should know + invite email required | P1 | Invite email is mandatory per notification-system.md (no stub in production) |
| Core HR | Employee profile updated | `employee.profile_updated` | No | Yes | No | No | No | `employees`, `audit_logs` | Employee lifecycle field change | P2 | Notification only for sensitive fields (e.g. bank details) if added later |
| Core HR | Employee transferred | `employee.transferred` | No | Yes | Yes | No | No | `employee_transfers`, `employee_assignment_history`, `employee_lifecycle_events`, `audit_logs`, `notifications` | Employee lifecycle + employee/new manager should know | P2 | Lightweight request record, not Workflow Engine |
| Core HR | Employee promoted | `employee.promoted` | No | Yes | Yes | No | No | `employee_lifecycle_events`, `employee_salary_history`, `audit_logs`, `notifications` | Employee lifecycle + employee should know | P2 | |
| Core HR | Salary changed | `employee.salary_changed` | No | Yes | Yes | No | No | `employee_salary_history`, `employee_lifecycle_events`, `audit_logs`, `notifications` | Compensation is admin-sensitive; employee should know | P1 | Notification should exclude amount details unless product decides otherwise |
| Core HR | Employee suspended | `employee.suspended` | No | Yes | Yes | No | No | `employees`, `employee_lifecycle_events`, `audit_logs`, `notifications` | Employee lifecycle, security-adjacent (access impact) | P1 | |
| Core HR | Employee terminated | `EmployeeTerminatedEvent` | **Yes** | Yes | Yes | No | No | `employees`, `employee_lifecycle_events`, `audit_logs`, `notifications` | Employee lifecycle with genuinely decoupled downstream reactions (access revocation, future workspace cleanup) | P1 | **Recommended first implementation slice — see Section 5** |
| Core HR | Employee offboarding started | `employee.offboarding_started` | No | Yes | Yes | No | No | `offboarding_records`, `employee_checklist_tasks`, `audit_logs`, `notifications` | Employee lifecycle + HR/IT checklist owners must act | P2 | |
| Core HR | Employee offboarding completed | `employee.offboarding_completed` | No | Yes | Yes | No | No | `offboarding_records`, `employee_lifecycle_events`, `audit_logs`, `notifications` | Employee lifecycle + HR should know | P2 | |
| Org Structure | Legal entity created/updated | `org.legal_entity_changed` | No | Yes | No | No | No | `legal_entities`, `audit_logs` | Tenant configuration, admin-sensitive | P2 | |
| Org Structure | Department created/updated | `org.department_changed` | No | Yes | No | No | No | `departments`, `audit_logs` | Tenant configuration, admin-sensitive | P2 | |
| Org Structure | Position created/updated | `org.position_changed` | No | Yes | No | No | No | `positions`, `audit_logs` | Tenant configuration, admin-sensitive | P2 | |
| Org Structure | Reporting line changed | `org.reporting_line_changed` | No | Yes | Yes | No | No | `position_reporting_history`, `positions`, `audit_logs`, `notifications` | Admin-sensitive + affected manager/employee should know | P2 | Also feeds `employee_hierarchy_closure` rebuild (not itself a trigger row) |
| Org Structure | Management coverage changed | `org.management_coverage_changed` | No | Yes | No | No | No | `management_coverage_records`, `audit_logs` | Admin-sensitive; affects alert routing but is config, not a user action | P2 | |
| Org Structure | Position access template changed | `org.position_access_template_changed` | No | Yes | No | No | No | `position_access_templates`, `audit_logs` | Permission-adjacent, admin-sensitive | P1 | Changes affect future `access_grant_requests` generation |
| Time Off | Time off request submitted | `time_off.requested` | No | Yes | Yes | Yes | No | `time_off_requests`, `audit_logs`, `notifications`, `email_delivery_logs` | User action requiring approver response; category `time_off` is In-app + Email per notification-system.md | P1 | Includes conflict count from `conflict_snapshot_json` |
| Time Off | Time off request approved | `time_off.approved` | No | Yes | Yes | Yes | No | `time_off_requests`, `time_off_entitlements`, `time_off_balances_audit`, `audit_logs`, `notifications`, `email_delivery_logs` | Balance change (admin-sensitive) + requester must know | P1 | |
| Time Off | Time off request rejected | `time_off.rejected` | No | Yes | Yes | Yes | No | `time_off_requests`, `audit_logs`, `notifications`, `email_delivery_logs` | Requester must know | P1 | |
| Time Off | Time off request cancelled | `time_off.cancelled` | No | Yes | Yes | No | No | `time_off_requests`, `time_off_entitlements`, `time_off_balances_audit`, `audit_logs`, `notifications` | Balance reversal (admin-sensitive) + approver may want to know | P2 | |
| Time Off | Time off balance adjusted | `time_off.balance_adjusted` | No | Yes | Yes | No | No | `time_off_balances_audit`, `time_off_entitlements`, `audit_logs`, `notifications` | Admin-sensitive manual adjustment + employee should know | P2 | |
| Time Off | Time off policy changed | `time_off.policy_changed` | No | Yes | No | No | No | `time_off_policies`, `time_off_policy_rules`, `time_off_policy_assignments`, `audit_logs` | Tenant configuration, admin-sensitive | P2 | |
| Time Off | Time off entitlement generated | `time_off.entitlement_generated` | No | No | No | No | No | `time_off_entitlements`, `time_off_balances_audit` | System-generated accrual bookkeeping, not a discrete user action | P3 | `time_off_balances_audit` row is the durable record; no separate audit_logs entry needed for routine accrual jobs |
| Calendar | Calendar event created/updated | `calendar.event_changed` | No | No | No | No | No | `calendar_events` | Simple CRUD; not security/lifecycle/billing-sensitive | P3 | Participant response tracked in `calendar_event_participants`, no trigger needed |
| Calendar | Calendar event cancelled | `calendar.event_cancelled` | No | No | Yes | No | No | `calendar_events`, `calendar_event_participants`, `notifications` | Participants must know | P2 | |
| Calendar | External calendar connected | `calendar.external_connected` | No | Yes | Yes | No | No | `calendar_events` (sync metadata), `audit_logs`, `notifications` | Third-party account link is security-adjacent, admin-sensitive | P2 | |
| Calendar | External calendar disconnected | `calendar.external_disconnected` | No | Yes | Yes | No | No | `calendar_events` (sync metadata), `audit_logs`, `notifications` | Security-adjacent disconnect | P2 | |
| Calendar | External calendar sync failed | `calendar.external_sync_failed` | No | Yes | Yes | No | No | `audit_logs`, `notifications` | User must know their calendar is out of sync | P2 | No live Google/Outlook call implemented in this step; trigger is documentation only |
| Calendar | External calendar event linked | `calendar.external_event_linked` | No | No | No | No | No | `calendar_events` | Simple CRUD/dedup via `external_id` | P3 | |
| Configuration | Tenant setting changed | `config.tenant_setting_changed` | No | Yes | No | No | No | `tenant_settings` (referenced), `audit_logs` | Tenant configuration, admin-sensitive | P1 | |
| Configuration | Monitoring feature toggle changed | `config.monitoring_toggle_changed` | No | Yes | Yes | No | No | `audit_logs`, `notifications` | Monitoring policy change, admin-sensitive + affected employees may need notice | P1 | |
| Configuration | App allowlist changed | `config.app_allowlist_changed` | No | Yes | No | No | No | `audit_logs` | Monitoring policy, admin-sensitive | P1 | |
| Configuration | Retention policy changed | `config.retention_policy_changed` | No | Yes | No | No | No | `audit_logs` | Tenant configuration, compliance-sensitive | P2 | |
| Configuration | Employee monitoring override changed | `config.employee_monitoring_override_changed` | No | Yes | Yes | No | No | `audit_logs`, `notifications` | Monitoring policy, admin-sensitive + employee should know | P2 | |
| Configuration | Work location setting changed | `config.work_location_setting_changed` | No | Yes | No | No | No | `legal_entities` (office fields), `audit_logs` | Tenant configuration, admin-sensitive | P2 | |
| Time & Attendance | Clock in | `attendance.clocked_in` | No | No | No | No | No | attendance/session tables | Simple CRUD, high-frequency; audit log would be noise | P1 | Feeds monitoring lifecycle gating per ADE-START-HERE.md, no trigger row itself |
| Time & Attendance | Clock out | `attendance.clocked_out` | No | No | No | No | No | attendance/session tables | Simple CRUD, high-frequency | P1 | |
| Time & Attendance | Break started | `attendance.break_started` | No | No | No | No | No | attendance/session tables | Simple CRUD | P2 | |
| Time & Attendance | Break ended | `attendance.break_ended` | No | No | No | No | No | attendance/session tables | Simple CRUD | P2 | |
| Time & Attendance | Attendance correction requested | `attendance.correction_requested` | No | Yes | Yes | No | No | attendance correction table, `audit_logs`, `notifications` | Approver must act; category `attendance` is In-app per notification-system.md | P2 | |
| Time & Attendance | Attendance correction approved/rejected | `attendance.correction_decided` | No | Yes | Yes | No | No | attendance correction table, `time_off_balances_audit` (if late-deduction reversed), `audit_logs`, `notifications` | Requester must know; may touch balances (admin-sensitive) | P2 | |
| Time & Attendance | Biometric event received | `attendance.biometric_event_received` | No | No | No | No | No | biometric event table | High-frequency device ingestion, simple CRUD | P2 | Audit only if it triggers a downstream security action |
| Time & Attendance | Late clock-in rule applied | `attendance.late_rule_applied` | No | Yes | No | No | No | `time_off_balances_audit` (`late_deduction`), `audit_logs` | Balance-affecting automated deduction, admin-sensitive for compliance trace | P2 | |
| Activity Monitoring | App allowlist violation detected | `AppAllowlistViolationDetectedEvent` | No (documented as future candidate only) | Yes | Yes | Yes (critical) | No | monitoring event table, `audit_logs`, `notifications`, `email_delivery_logs` | Monitoring policy violation, admin-sensitive + recipient must act; category `monitoring` is In-app + Email (critical) | P1 | Recipient resolved via management coverage chain per notification-system.md, not "reporting manager" by default |
| Activity Monitoring | Idle threshold exceeded | `monitoring.idle_threshold_exceeded` | No | Yes | Yes | Yes (critical) | No | monitoring event table, `audit_logs`, `notifications`, `email_delivery_logs` | Same as above | P2 | |
| Activity Monitoring | Screenshot captured | `monitoring.screenshot_captured` | No | No | No | No | No | screenshot table | High-frequency evidence capture, simple CRUD | P3 | Not itself a trigger; only violations built from it are |
| Activity Monitoring | Activity snapshot recorded | `monitoring.activity_snapshot_recorded` | No | No | No | No | No | `activity_snapshots` (append-only) | High-frequency append-only ingestion | P3 | |
| Activity Monitoring | Daily activity summary generated | `monitoring.daily_summary_generated` | No | No | No | No | No | daily summary table | Scheduled aggregation job output, not a discrete user action | P3 | |
| Identity Verification | Verification requested | `verification.requested` | No | No | Yes | No | No | verification table, `notifications` | Employee must act (submit photo/biometric) | P2 | |
| Identity Verification | Verification passed | `verification.passed` | No | No | No | No | No | verification table | Simple CRUD outcome; no action needed | P3 | |
| Identity Verification | Verification failed | `verification.failed` | No | Yes | Yes | Yes (critical) | No | verification table, `audit_logs`, `notifications`, `email_delivery_logs` | Monitoring-adjacent policy failure, admin-sensitive + recipient must act | P1 | category `monitoring` per notification-system.md |
| Identity Verification | Verification expired | `verification.expired` | No | Yes | Yes | Yes (critical) | No | verification table, `audit_logs`, `notifications`, `email_delivery_logs` | Same rationale as failed | P2 | |
| Identity Verification | Biometric enrollment completed | `verification.biometric_enrollment_completed` | No | Yes | Yes | No | No | biometric enrollment table, `audit_logs`, `notifications` | Security-sensitive enrollment + employee should know | P2 | |
| Identity Verification | Biometric audit event recorded | `verification.biometric_audit_event_recorded` | No | Yes | No | No | No | `audit_logs` | Security/compliance trace | P2 | |
| Discrepancy Engine | Discrepancy detected | `discrepancy.detected` | No | Yes | Yes | No | No | discrepancy table, `audit_logs`, `notifications` | Monitoring-adjacent, admin-sensitive + recipient must review | P2 | |
| Discrepancy Engine | Discrepancy marked critical/high | `discrepancy.severity_escalated` | No | Yes | Yes | Yes (critical) | No | discrepancy table, `audit_logs`, `notifications`, `email_delivery_logs` | category `discrepancy` is In-app + Email (critical) per notification-system.md | P1 | |
| Discrepancy Engine | Discrepancy resolved | `discrepancy.resolved` | No | Yes | Yes | No | No | discrepancy table, `audit_logs`, `notifications` | Admin-sensitive closure + requester/reporter should know | P2 | |
| Notifications | Notification created | *(pipeline step, not a business trigger)* | No | No | No | No | No | `notifications` | This is the Track step of the delivery pipeline itself, not a separate business action to instrument | P3 | Every other row's "Notification? Yes" already implies this write |
| Notifications | Email delivery attempted | *(pipeline step, not a business trigger)* | No | No | No | Yes | No | `email_delivery_logs` | Track step for the email channel | P3 | Row created before dispatch per notification-system.md |
| Notifications | Email delivery failed | `notification.email_delivery_failed` | No | Yes | No | No | No | `email_delivery_logs`, `audit_logs` | Delivery failure needs an operational trace (admin-sensitive troubleshooting), not a new user notification loop | P2 | Retry creates a new `email_delivery_logs` row, does not overwrite |
| Notifications | Notification read | *(pipeline step, not a business trigger)* | No | No | No | No | No | `notifications` | Simple state flip (`is_read`, `read_at`); no audit/notify/email/webhook value | P3 | |
| Notifications | Notification action completed | `notification.action_completed` | No | No | No | No | No | `notifications`, target entity | Belongs to the underlying command (e.g. approving the linked request), not a separate trigger | P3 | Avoid double-instrumenting: the underlying action (e.g. `time_off.approved`) is the real trigger |
| Work Management | Workspace created | `work.workspace_created` | No | No | No | No | No | workspace table | Simple CRUD | P2 | |
| Work Management | Workspace member added/removed | `work.workspace_member_changed` | No | No | Yes | No | No | workspace member table, `notifications` | New member should know they were added | P3 | |
| Work Management | Project created/updated | `work.project_changed` | No | No | No | No | No | project table | Simple CRUD | P2 | |
| Work Management | Project member added/removed | `work.project_member_changed` | No | No | Yes | No | No | project member table, `notifications` | Member should know | P3 | |
| Work Management | Task created | `work.task_created` | No | No | No | No | No | task table | Simple CRUD | P2 | |
| Work Management | Task assigned | `work.task_assigned` | No | No | Yes | No | No | task assignment table, `notifications` | Assignee must know | P1 | |
| Work Management | Task status changed | `work.task_status_changed` | No | No | Yes | No | No | task table, `notifications` | Watchers/assignee should know on meaningful transitions (e.g. done) | P2 | Avoid firing on every minor status edit; scope to key transitions |
| Work Management | Task approved/rejected | `work.task_decision` | No | No | Yes | No | No | `task_approvals`, `notifications` | Requester must know outcome | P2 | |
| Work Management | Task comment added | `work.task_comment_added` | No | No | Yes | No | No | task comment table, `notifications` | Mentioned/assigned users should know | P3 | |
| Work Management | Task document attached | `work.task_document_attached` | No | No | No | No | No | `task_documents` | Simple CRUD | P3 | |
| Work Management | Worklog created/updated | `work.worklog_changed` | No | No | No | No | No | `time_logs` | Simple CRUD | P3 | |
| Work Management | Timesheet submitted | `work.timesheet_submitted` | No | No | Yes | No | No | `timesheets`, `notifications` | Approver must act | P2 | |
| Work Management | Timesheet approved/rejected | `work.timesheet_decided` | No | No | Yes | No | No | `timesheets`, `notifications` | Requester must know | P2 | |
| Work Management | Document published | `work.document_published` | No | No | Yes | No | No | document table, `notifications` | category `documents` is In-app + Email in full spec, but email is Phase 2-scope content workflow here; Phase 1 test scope keeps in-app only | P3 | |
| Work Management | Document version created | `work.document_version_created` | No | No | No | No | No | `document_versions` | Simple CRUD | P3 | |
| Work Management | Document approval requested/approved/rejected | `work.document_approval_decision` | No | No | Yes | No | No | `document_approvals`, `notifications` | Approver/requester must know | P2 | |
| Work Management | Wiki page created/updated | `work.wiki_page_changed` | No | No | No | No | No | `wiki_pages` | Simple CRUD | P3 | |
| Shared Platform | Subscription created | `platform.subscription_created` | No | Yes | Yes | No | No | subscription table, `audit_logs`, `notifications` | Billing-sensitive + tenant owner should know | P1 | |
| Shared Platform | Subscription plan changed | `platform.subscription_plan_changed` | No | Yes | Yes | No | No | subscription table, `audit_logs`, `notifications` | Billing-sensitive + tenant owner should know | P1 | |
| Shared Platform | Subscription add-on/resource add-on changed | `platform.subscription_addon_changed` | No | Yes | Yes | No | No | subscription add-on table, `audit_logs`, `notifications` | Billing-sensitive | P2 | |
| Shared Platform | Invoice created | `platform.invoice_created` | No | Yes | Yes | Yes | No | invoice table, `audit_logs`, `notifications`, `email_delivery_logs` | Billing-sensitive + tenant owner must know + receipt by email | P1 | |
| Shared Platform | Invoice paid | `platform.invoice_paid` | No | Yes | Yes | Yes | No | invoice table, `audit_logs`, `notifications`, `email_delivery_logs` | Billing-sensitive confirmation | P1 | |
| Shared Platform | Invoice failed | `platform.invoice_failed` | No | Yes | Yes | Yes | No | invoice table, `audit_logs`, `notifications`, `email_delivery_logs` | Billing-sensitive + tenant owner must act | P1 | |
| Shared Platform | Tenant provisioning started | `platform.tenant_provisioning_started` | No | Yes | No | No | No | `tenants`, `audit_logs` | Admin-sensitive platform operation | P2 | Developer Platform internal, not tenant-user-facing |
| Shared Platform | Tenant provisioning completed | `platform.tenant_provisioning_completed` | No | Yes | Yes | Yes | No | `tenants`, `audit_logs`, `notifications`, `email_delivery_logs` | Tenant owner must know their tenant is ready | P1 | |
| Shared Platform | Tenant provisioning failed | `platform.tenant_provisioning_failed` | No | Yes | Yes | No | No | `tenants`, `audit_logs`, `notifications` | Admin-sensitive failure needs operator visibility | P2 | |
| Shared Platform | Configuration template applied | `platform.configuration_template_applied` | No | Yes | No | No | No | `audit_logs` | Tenant configuration, admin-sensitive | P2 | |
| Shared Platform | Support ticket created | `platform.support_ticket_created` | No | No | Yes | Yes | No | `support_tickets`, `notifications`, `email_delivery_logs` | Support agent must be alerted; confirmation email to requester | P2 | |
| Shared Platform | Support ticket replied | `platform.support_ticket_replied` | No | No | Yes | Yes | No | `support_tickets`, `notifications`, `email_delivery_logs` | Other party must know | P2 | |
| Shared Platform | Support ticket resolved | `platform.support_ticket_resolved` | No | No | Yes | No | No | `support_tickets`, `notifications` | Requester must know | P2 | |
| Shared Platform | Webhook endpoint created/updated | `platform.webhook_endpoint_changed` | No | Yes | No | No | No | `webhook_endpoints`, `audit_logs` | Admin-sensitive integration configuration | P2 | |
| Shared Platform | Webhook delivery attempted | *(delivery mechanism, not a business trigger)* | No | No | No | No | Yes | `webhook_deliveries` | This row is the webhook delivery itself, fired only for tenant-subscribed event types in `webhook_endpoints.events` | P2 | Not implemented in this step; documentation only per non-goals |
| Shared Platform | Webhook delivery failed | `platform.webhook_delivery_failed` | No | Yes | No | No | Yes | `webhook_deliveries`, `audit_logs` | Admin-sensitive integration troubleshooting | P3 | |
| Shared Platform | Rate limit rule changed | `platform.rate_limit_rule_changed` | No | Yes | No | No | No | rate limit table, `audit_logs` | Tenant/platform configuration, admin-sensitive | P3 | |
| Agent Gateway | Agent registered | `agent.registered` | No | Yes | Yes | No | No | `registered_agents` (or equivalent), `audit_logs`, `notifications` | Security-sensitive device registration + admin should know | P2 | |
| Agent Gateway | Agent session started/ended | `agent.session_changed` | No | No | No | No | No | agent session table | High-frequency, simple state tracking | P3 | |
| Agent Gateway | Agent command issued | `agent.command_issued` | No | Yes | No | No | No | agent command table, `audit_logs` | Remote command execution is admin-sensitive | P2 | |
| Agent Gateway | Agent command completed | `agent.command_completed` | No | No | No | No | No | agent command table | Simple CRUD outcome unless it fails | P3 | |
| Agent Gateway | Agent command failed | `agent.command_failed` | No | Yes | Yes | No | No | agent command table, `audit_logs`, `notifications` | Admin-sensitive failure needs operator visibility | P2 | |
| Agent Gateway | Agent health issue detected | `agent.health_issue_detected` | No | Yes | Yes | No | No | agent health table, `audit_logs`, `notifications` | Admin-sensitive + IT/admin should know | P2 | |
| Agent Gateway | Agent policy changed | `agent.policy_changed` | No | Yes | No | No | No | agent policy table, `audit_logs` | Monitoring policy change, admin-sensitive | P1 | |
| Agent Gateway | Work location mismatch detected | `monitoring.work_location_mismatch_detected` | No | Yes | Yes | Yes (critical) | No | work location evidence table, `audit_logs`, `notifications`, `email_delivery_logs` | category `monitoring` In-app + Email (critical) per notification-system.md | P1 | Recipient resolved via management coverage chain, same as other monitoring alerts |
| Developer Platform | Platform login succeeded | `devplatform.login_succeeded` | No | Yes | No | No | No | `platform_users` (session), `audit_logs` | Security event on internal console | P2 | |
| Developer Platform | Platform login failed | `devplatform.login_failed` | No | Yes | No | No | No | `audit_logs` | Security event | P2 | |
| Developer Platform | Platform role changed | `devplatform.role_changed` | No | Yes | Yes | No | No | platform role table, `audit_logs`, `notifications` | Permission change, admin-sensitive | P2 | |
| Developer Platform | Platform permission changed | `devplatform.permission_changed` | No | Yes | No | No | No | platform permission table, `audit_logs` | Permission change, admin-sensitive | P2 | |
| Developer Platform | Demo request submitted | `devplatform.demo_request_submitted` | No | No | Yes | Yes | No | demo request table, `notifications`, `email_delivery_logs` | Sales/ops must act; confirmation email to requester | P3 | |
| Developer Platform | Demo request approved/rejected | `devplatform.demo_request_decided` | No | Yes | Yes | Yes | No | demo request table, `audit_logs`, `notifications`, `email_delivery_logs` | Leads to tenant provisioning (admin-sensitive) + requester must know | P3 | |
| Developer Platform | Demo profile changed | `devplatform.demo_profile_changed` | No | No | No | No | No | demo profile table | Simple CRUD | P3 | |

**Total trigger rows in matrix: 95**

---

## 5. Recommended First Implementation Slice

**`EmployeeTerminatedEvent`**

Why this slice and not any other row above:

- It is a meaningful, unambiguous business event (not CRUD) — an employee's `employment_status` transitions to `terminated`, which is a one-way, high-consequence state change.
- It sits squarely in employee lifecycle, which the classification rules in Section 3 already mark as requiring an audit log.
- It needs a notification (HR/admin should be told a termination was recorded).
- It is the canonical example in `domain-events.md` itself (`EmployeeTerminatedEvent triggers access revocation, workspace cleanup, and notifications`), so it is already a validated, decoupled use case rather than a manufactured one.
- The access-revocation reaction can be added later as a second handler without touching the original termination command — this is what actually justifies a domain event under the "one or more secondary reactions... command should not need to know every downstream reaction" rule, unlike most other rows in the matrix which are handled directly in-command.
- It can later extend to workspace/task cleanup (Work Management) without coupling Core HR to Work Management.
- It is fully testable without any external integration (no email/webhook/live provider call required for the slice itself).

Planned future files (not created in this step):

Future event:
```
C:\onevoNew\test\backend\Models\Employees\Events\EmployeeTerminatedEvent.cs
```

Future handlers:
```
C:\onevoNew\test\backend\Services\EventHandlers\Employees\WriteAuditLogOnEmployeeTerminatedHandler.cs
C:\onevoNew\test\backend\Services\EventHandlers\Employees\NotifyHrOnEmployeeTerminatedHandler.cs
C:\onevoNew\test\backend\Services\EventHandlers\Employees\RevokeAccessOnEmployeeTerminatedHandler.cs
```

Future tests:
- Terminating an employee creates an `employee_lifecycle_events` row.
- Terminating an employee creates an `audit_logs` row.
- Terminating an employee creates a `notifications` row.
- No event fires if the termination save fails (event dispatch only happens after `SaveChangesAsync` succeeds, per `domain-events.md`).
- Event handlers do not use `AppDbContext` directly if a repository pattern exists in this test backend (mirrors the OneVo-HR persistence-access rule for event handlers).

---

## 6. Explicit Non-Goals

- No event bus.
- No external integration event contracts.
- No RabbitMQ.
- No MassTransit.
- No outbox publishing system.
- No Workflow/Automation Engine.
- No event for every table.
- No live email/webhook/provider calls.

---

## 7. Verification

This step adds documentation only; no source files were created or modified under `backend/`, `backend.Tests/`, or `onevo-tenant-app/`. Expected result: all three commands below pass exactly as they did before this step, since nothing they compile or run has changed.

```
cd C:\onevoNew\test\backend
dotnet build

cd C:\onevoNew\test\backend.Tests
dotnet test

cd C:\onevoNew\test\onevo-tenant-app
npm run build
```
