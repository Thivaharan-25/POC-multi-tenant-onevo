# Agent Service/TrayApp Split + JWT Device Auth — Design

**Goal:** Rearchitect the tray application from a single MAUI app that talks to the backend directly into the documented production architecture: a background `ONEVO.Agent.Service` (Windows Service, owns all business logic and backend communication) plus a thin `ONEVO.Agent.TrayApp` (MAUI UI, talks only to the Service over Named Pipes). Switch device authentication from an opaque hashed token to a signed JWT device credential, matching `modules/agent-gateway/*` in the `OneVo-HR-2ndbrain-16.06.2026` vault.

**Source of truth:** `modules/agent-gateway/agent-overview.md`, `ipc-protocol.md`, `agent-server-protocol.md`, `agent-registration/`, `data-ingestion/`. Where `backend/agent/*` in the same vault contradicts these (it describes an older WinForms/machine-token design), `modules/agent-gateway/*` wins — it is more detailed and internally consistent.

**Explicitly out of scope for this spec** (future specs):
- MSI/MSIX installer packaging (build and manually test the Service first, per your instruction — installer comes after).
- SQLite offline buffer, heartbeat (`POST /heartbeat`), tamper resistance, `DetectOfflineAgentsJob`.
- Photo verification / `capture_photo` IPC messages.
- Full multi-type `ingest` batch (`meeting`, `screenshot_capture`, `communication_usage`, `document_usage`, `verification_photo`) — only `app_usage` is wired.
- Workforce Presence module / event-driven `StartMonitoring`/`StopMonitoring` via SignalR — clock-in/out stays our own pragmatic REST endpoint (not in the docs, which assume a separate Workforce Presence module publishing domain events).
- macOS agent (explicitly Phase 2 in the docs).

**Components touched:**
- Backend: `poc/POC-multi-tenant-onevo/backend`.
- Desktop: `OneVoTray app/` — new `ONEVO.Agent.Service` project, `OneVoTray` renamed to `ONEVO.Agent.TrayApp`, `ONEVO.Agent.Shared` filled in.

---

## 1. Architecture Overview

```
ONEVO.Agent/                          (solution root)
├── ONEVO.Agent.Service/              (NEW — Windows Service, business logic)
│   ├── Collectors/                   (app-usage foreground-window polling — moved from TrayApp)
│   ├── Sync/                         (ingest upload to backend)
│   ├── Security/                     (JWT storage via DPAPI/Credential Manager)
│   └── IPC/NamedPipeServer.cs
├── ONEVO.Agent.TrayApp/              (RENAMED from OneVoTray — thin UI client)
│   ├── Views/ (LoginWindow etc.)
│   └── Services/NamedPipeClient.cs
└── ONEVO.Agent.Shared/               (already exists)
    ├── IPC/IpcMessages.cs            (currently empty — filled in)
    └── Constants.cs                  (PipeName = "onevo-agent-ipc")
```

**Data flow:**
```
TrayApp "Sign in" → IPC → Service → POST /api/v1/agent/enroll/start (backend)
                                   → browser SSO (TrayApp opens auth_url)
Service → POST /api/v1/agent/enroll/complete → device JWT (claims: device_id, tenant_id, type=agent)
Service stores JWT (DPAPI/Credential Manager)
Service → status_update (IPC) → TrayApp shows "Enrolled"

TrayApp "Clock In" → IPC → Service
Service starts foreground-window polling → buffers → POST /api/v1/agent/ingest (Bearer JWT)
Service → status_update (IPC) → TrayApp shows "Monitoring: Active"
```

---

## 2. Backend

### 2.1 Renamed/new endpoints (`api/v1/agent`)

| Endpoint | Auth | Notes |
|---|---|---|
| `POST /enroll/start` | anonymous | Replaces `/api/v1/devices/pair`. Body: `device_id, device_name, os_version, agent_version, enrollment_method`. Returns `enrollment_id, auth_url, expires_at`. |
| `GET /enroll/status?enrollment_id=...` | anonymous | Polling equivalent of our existing `/pair/status` — returns confirmation state, and once confirmed, an `authorization_code` for `enroll/complete`. |
| `POST /enroll/complete` | anonymous (proof of possession = `authorization_code`) | Replaces `/pair/consent`. Body: `enrollment_id, device_id, authorization_code`. Returns `agent_id, tenant_id, employee_id, employee_name, device_token (JWT), token_expires_at, policy`. |
| `POST /ingest` | Bearer JWT | Replaces `/app-usage`. Body: `device_id, employee_id, timestamp, batch: [{ type: "app_usage", data: {...} }]`. Returns `202`. |
| `POST /clock-in`, `POST /clock-out` | Bearer JWT | Kept as our own pragmatic addition — not in the docs (Workforce Presence module territory, not built). |

**Consolidation from our existing implementation:** docs' `enroll/start` returns only `auth_url`, no separate short human-code. Our tested flow uses a short `userCode` (displayed + "Copy Code" fallback) plus a private polling code. We consolidate to **one `enrollment_id`** — used both in the auto-opened `auth_url` and for polling — but still display it in the UI for the copy-code fallback, preserving the original mockup's UX within the docs' single-ID model.

### 2.2 JWT device token
- New `ITokenService.GenerateDeviceTokenAsync(deviceId, tenantId)` — HMAC-SHA256-signed JWT. Claims: **exactly** `device_id`, `tenant_id`, `type = "agent"` (no `employee_id` claim — the docs are explicit that employee identity is resolved server-side from `registered_agents`/`agent_sessions`, never trusted from the token or payload).
- Signing key: symmetric HMAC key in configuration (POC-appropriate; the docs don't specify production key management).
- Expiry: **90 days** (not specified in docs; chosen to match the "device stays enrolled permanently" framing — re-enrollment is the recovery path for an expired/revoked credential, not a refresh-token flow, which the docs also don't fully specify).
- `DeviceTokenAuthMiddleware` updated to validate JWT signature + expiry instead of the current hash-lookup, then resolves `RegisteredAgent` by `device_id` claim to get `EmployeeId`.

### 2.3 Migration safety
Add the new `/enroll/*` and `/ingest` routes alongside the existing `/pair/*` and `/app-usage` routes. Only remove the old routes once the new Service + TrayApp are verified end-to-end against them — avoids a window where nothing works.

---

## 3. `ONEVO.Agent.Service` (new project)

- .NET Worker Service (`Microsoft.Extensions.Hosting.BackgroundService`), `net10.0-windows`.
- Hosts `NamedPipeServerStream` on pipe `onevo-agent-ipc` (`PipeDirection.InOut`, 1 server instance, byte mode, async).
- Owns everything currently in `LoginWindowViewModel`: enrollment state machine, `OnevoApiClient` (HTTP calls to backend), `AppUsageTrackingService` (moved as-is from TrayApp), `CredentialStore` (now stores the JWT instead of an opaque token).
- Dev/testing: registered manually via `sc create` (installer deferred, per your instruction — Service code first, then MSI, then test machine, then client rollout, as you described).

---

## 4. `ONEVO.Agent.TrayApp` (renamed from `OneVoTray`)

- Becomes a **thin UI client** — `LoginWindow`/`LoginWindowViewModel` no longer call `OnevoApiClient` or `CredentialStore` directly. They send IPC commands to the Service and render whatever `status_update` messages arrive.
- All business/network logic removed from this project.

---

## 5. IPC Protocol (`ONEVO.Agent.Shared`)

- `Constants.PipeName = "onevo-agent-ipc"`.
- Transport: `System.IO.Pipes`, JSON objects, one per line (`\n`-delimited), camelCase, enums as strings.
- Envelope: `IpcMessage { Type, MessageId (guid), Timestamp }` base record.
- Messages implemented now:
  - TrayApp → Service: `employee_login` (our sign-in trigger), `get_status`.
  - Service → TrayApp: `status_update` (Step/UserCode/EmployeeName/IsClockedIn — mirrors today's `LoginWindowViewModel` observable properties), `policy_updated` (received but not yet acted on — policy enforcement is future work).
- Deferred: `capture_photo`, `photo_captured`, `verification_result` (photo verification not built).
- Reconnection: TrayApp retries connecting every 2000ms if the Service is unreachable. Per docs, **no message queuing during disconnection** — a command sent while disconnected is simply lost; TrayApp resyncs via `get_status` on reconnect.

---

## 6. Migration Plan

1. **Backend first** (additive): implement JWT issuance + new `/enroll/*` and `/ingest` routes alongside the existing ones.
2. **`ONEVO.Agent.Shared`**: implement `IpcMessages.cs` + `Constants.PipeName` (currently empty stubs, scaffolded exactly for this).
3. **New `ONEVO.Agent.Service` project**: move `OnevoApiClient`, `AppUsageTrackingService`, `CredentialStore`, and the enrollment/clock-in state machine out of `LoginWindowViewModel` into it, adapted to be IPC-driven.
4. **Rename `OneVoTray` → `ONEVO.Agent.TrayApp`**, strip business logic, wire up the (currently empty) `NamedPipeClient`, rebuild `LoginWindowViewModel` as a thin view over IPC status messages.
5. **Re-run the full E2E test** (curl + real browser + real Service + real TrayApp) already validated once against the old architecture, against the new one.
6. Remove the old `/pair/*` and `/app-usage` backend routes once step 5 passes.

---

## 7. Error Handling

| Case | Behavior |
|---|---|
| TrayApp can't connect to Service (not running/installed) | Retry every 2000ms; TrayApp shows "Agent service not running." |
| Pipe disconnects mid-session | No message queuing (per docs) — TrayApp reconnects and calls `get_status` to resync state. |
| JWT expired/invalid at `/ingest` time | 401 → Service transitions to re-enrollment, sends `status_update` telling TrayApp to show "Sign in required" again. |
| Service crashes | Windows SCM auto-restart; in-memory app-usage buffer for the current interval is lost (same limitation as today — full SQLite offline buffer is future work). |

---

## 8. Testing
- Backend: unit tests for `ITokenService` (JWT generation/validation, expiry), integration tests for `/enroll/start`, `/enroll/complete`, `/ingest` against the `backend.Tests` patterns already in use.
- IPC: manual verification (Service + TrayApp running side by side) — no automated IPC test harness in this phase.

---

## 9. Key Decisions Log
- `enrollment_id` **consolidates** our earlier two-code (`userCode` + `deviceCode`) design into the docs' single-ID model, while keeping the copy-code UI fallback from the original mockup.
- JWT expiry set to 90 days — not specified in the docs; chosen to match "device stays enrolled permanently."
- Clock-in/out endpoints are a deliberate, documented deviation from the docs (which assume a separate, unbuilt Workforce Presence module driving monitoring via events).
- Backend routes migrate additively (old + new side by side) until the new Service/TrayApp are verified, then old routes are removed — avoids a broken intermediate state.
- Installer (MSI) explicitly deferred until Service code is complete and manually tested, per your stated rollout process (dev → publish → MSI → test machine → tester → production).
