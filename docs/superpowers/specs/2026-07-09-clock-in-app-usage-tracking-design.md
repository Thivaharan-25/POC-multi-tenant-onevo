# Minimal Clock In/Out + Application Usage Tracking — Design

**Goal:** Let the enrolled tray app (OneVoTray) clock an employee in/out and, while clocked in, periodically report which application/window is in the foreground so the backend can build per-app-per-day usage totals. This is the first slice of the "WorkPulse Agent" activity-monitoring product (mockup steps 8–9), scoped down to the simplest end-to-end pipeline.

**Explicitly out of scope for this spec** (future specs):
- Screenshot capture (`MonitoringEvidenceAsset`).
- Mouse/keyboard input tracking (`ActivitySnapshot`/`ActivityRawBuffer`).
- Photo verification at clock-in.
- The full schedule-aware `AttendanceRecord`/`PresenceSession` attendance module — this spec adds a minimal, purpose-built clock state instead of wiring into that larger, not-yet-built system.
- `MonitoringFeatureToggle`-gated enable/disable (tenant policy toggles) — tracking runs unconditionally whenever clocked in, for this first slice.

**Components touched:**
- Backend: `poc/POC-multi-tenant-onevo/backend`.
- Desktop: `OneVoTray app/OneVoTray`.

---

## 1. Architecture Overview

```
Tray app (background timer, every 10s)          Backend
   │ sample foreground process/window             │
   │ (local buffer, flushed every 60s)             │
   │                                                │
   │  POST /api/v1/agent/clock-in                  │
   │  Authorization: Bearer <deviceToken> ─────────▶│  verify DeviceTokenHash → resolve Tenant/Employee
   │                                                │  create/update AgentClockState (IsClockedIn=true)
   │                                                │
   │  every 60s while clocked in:                  │
   │  POST /api/v1/agent/app-usage (batch) ────────▶│  reject (409) if not clocked in
   │                                                │  upsert ApplicationUsage rows (per app, per day)
   │                                                │
   │  POST /api/v1/agent/clock-out ────────────────▶│  IsClockedIn=false
```

**New backend auth path:** `Authorization: Bearer <deviceToken>` is validated against `registered_agents.DeviceTokenHash` (hashed with the same `AuthService.HashToken` used everywhere else). This is separate from the existing cookie-session path and only applies to `/api/v1/agent/*`.

---

## 2. Backend

### 2.1 New entity: `AgentClockState`

One row per `RegisteredAgent`, updated in place (not an event log).

| Field | Type | Notes |
|---|---|---|
| `Id` | Guid | PK |
| `RegisteredAgentId` | Guid | FK to `registered_agents`, unique |
| `TenantId` | Guid | |
| `EmployeeId` | Guid? | |
| `IsClockedIn` | bool | |
| `ClockedInAt` | DateTimeOffset? | |
| `ClockedOutAt` | DateTimeOffset? | |

Deliberately not the full `AttendanceRecord`/`PresenceSession` schema (schedule-aware, belongs to a separate attendance module this spec doesn't build).

### 2.2 New `DeviceTokenAuthContext`

A small scoped service, resolved by a new middleware (`DeviceTokenAuthMiddleware`) that runs only for `/api/v1/agent/*`:
- Reads `Authorization: Bearer <token>`.
- Hashes it, looks up `RegisteredAgent` by `DeviceTokenHash`.
- On match, populates `DeviceTokenAuthContext` with `RegisteredAgentId`, `TenantId`, `EmployeeId`.
- On no match, returns 401 and short-circuits (mirrors how `AuthBoundaryMiddleware` already short-circuits for the cookie-session path).
- `/api/v1/agent/*` is added to the `AuthBoundaryMiddleware` exemption list (alongside `/api/v1/auth` and `/api/v1/devices/pair`) since it uses this new auth path instead of the cookie-session one.

### 2.3 New `AgentController` (`api/v1/agent`)

| Endpoint | Purpose |
|---|---|
| `POST /clock-in` | Creates/updates `AgentClockState` (`IsClockedIn=true`, `ClockedInAt=now`). 409 if already clocked in. Returns `{ isClockedIn, clockedInAt }`. |
| `POST /clock-out` | Sets `IsClockedIn=false`, `ClockedOutAt=now`. 409 if not clocked in. Returns `{ isClockedIn, clockedOutAt }`. |
| `POST /app-usage` | Body: `{ samples: [{ applicationName, processName, windowTitleHash, date, seconds }] }`. 409 if not currently clocked in. Upserts each sample into `ApplicationUsage` (adds `seconds` to the existing `TotalSeconds` for that tenant+employee+date+applicationName row, or creates one). Returns `204`. |

`windowTitleHash` reuses `ApplicationUsage`'s existing hashed-title column — the tray app hashes locally (same `SHA256` approach as `AuthService.HashToken`); the raw window title is never sent over the wire.

---

## 3. Desktop (`OneVoTray`)

### 3.1 New `Services/AppUsageTrackingService.cs`
- P/Invoke `GetForegroundWindow` + `GetWindowThreadProcessId` (user32.dll) every 10s to identify the active process/window.
- Aggregates samples locally into a per-app-per-day seconds total.
- Every 60s, flushes the local aggregate to `POST /api/v1/agent/app-usage` via `OnevoApiClient` (extended with the `Authorization: Bearer <deviceToken>` header, read from `CredentialStore`).
- On upload failure, keeps the local aggregate and retries next interval instead of dropping data.
- Only runs between Clock In and Clock Out.

### 3.2 `LoginWindow` / `LoginWindowViewModel` changes
- `Enrolled` step gains **Clock In** / **Clock Out** buttons (no photo verification — deferred) and a status line.
- `ClockInCommand` calls `POST /clock-in`, starts `AppUsageTrackingService`.
- `ClockOutCommand` calls `POST /clock-out`, stops `AppUsageTrackingService` (flushing any remaining buffered data first).

### 3.3 `OnevoApiClient` changes
- New `ClockInAsync`/`ClockOutAsync`/`SubmitAppUsageAsync` methods, all sending `Authorization: Bearer <deviceToken>`.

---

## 4. Error Handling

| Case | Behavior |
|---|---|
| Bearer token invalid/unknown | 401 — tray app surfaces a re-auth prompt (rare; would mean the stored credential is stale/revoked). |
| App-usage upload while not clocked in | 409 — tray app stops its local timer instead of retrying indefinitely. |
| Network failure during batch upload | Keep the batch in the local buffer, retry on the next interval. |
| Clock-in when already clocked in (e.g. app restarted mid-session) | 409 — tray app treats this as already-clocked-in and proceeds normally rather than showing an error. |

---

## 5. Testing
- Backend: unit tests for the Bearer-token resolution and `AgentController` state transitions (clock-in/out, app-usage upsert, 409 cases), following existing `backend.Tests` patterns.
- Desktop: manual verification (no automated test project for `OneVoTray` yet).

---

## 6. Key Decisions Log
- A dedicated Bearer-`deviceToken` auth path is added for `/api/v1/agent/*`, kept fully separate from the cookie-session path used by the browser.
- Clock state is a new minimal `AgentClockState` row, not the full schedule-aware `AttendanceRecord`/`PresenceSession` module.
- App-usage samples are aggregated client-side into per-app-per-day totals before upload, not streamed as raw per-second events.
- Screenshot capture and input (mouse/keyboard) tracking are separate future specs.
