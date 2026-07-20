# Tray App Device Pairing & Authorization — Design

**Goal:** Let the OneVoTray desktop app (WorkPulse Agent) authenticate itself against a specific tenant/employee in the OneVo HRMS platform without ever handling the user's password, using a browser-delegated device pairing flow — the same mechanism GitHub CLI / gcloud / OAuth Device Authorization Grant (RFC 8628) use. This corresponds to steps 4–7 of the WorkPulse Agent product mockup ("Tray Icon – Sign In Required" → "Connect TrayApp to OneVo HRMS" → "Monitoring Notice" → "Device Enrolled").

**Explicitly out of scope for this spec** (future specs):
- HRMS web onboarding banner ("Device setup required") and the agent download page (mockup steps 1–2).
- MSI installer packaging (mockup step 3).
- Clock-in, photo verification, active monitoring, screenshots, and the monitoring settings panel (mockup steps 8–9).

**Components touched:**
- Backend: `poc/POC-multi-tenant-onevo/backend` (OnevoHr.Api, ASP.NET Core + EF Core, cookie-session auth, multi-tenant).
- Frontend: `poc/POC-multi-tenant-onevo/onevo-tenant-app` (Angular, standalone components).
- Desktop: `OneVoTray app/OneVoTray` (.NET MAUI Windows tray app) + `OneVoTray app/ONEVO.Agent.Shared`.

**Tech stack:** ASP.NET Core + EF Core, Angular standalone components, .NET MAUI (net10.0-windows), CommunityToolkit.Mvvm.

---

## 1. Architecture Overview

The flow mirrors the OAuth 2.0 Device Authorization Grant. The tray app never sees the user's password — the user authenticates in their normal, already-logged-in browser session.

```
Tray app                    Backend                     Browser (already logged in)
   │  POST /devices/pair        │                                │
   ├────────────────────────────▶  generates userCode+deviceCode │
   │  {userCode, deviceCode}    │                                │
   ◀────────────────────────────┤                                │
   │  shows userCode,           │                                │
   │  opens browser ────────────┼───────────────────────────────▶│  /devices/confirm?code=userCode
   │                            │  GET /devices/pair/{userCode}   │
   │                            ◀──────────────────────────────────┤ (session cookie auth)
   │                            │  shows employee/device, Confirm │
   │                            │  POST .../confirm ──────────────┤
   │                            │  binds TenantId+EmployeeId      │
   │  poll GET .../status       │                                │
   ├────────────────────────────▶ Status=Confirmed                │
   ◀────────────────────────────┤                                │
   │  shows Monitoring Notice   │                                │
   │  POST .../consent          │                                │
   ├────────────────────────────▶ Status=Enrolled, issues deviceToken│
   ◀────────────────────────────┤                                │
   │  stores token (Windows Credential Manager)                   │
   │  shows "Device Enrolled"   │                                │
```

Key property: the device code (`userCode`) is generated **before** the backend knows which tenant is involved. Tenant/employee binding happens only when the already-authenticated browser session confirms the code — the tray app never needs to ask the user for a company domain.

---

## 2. Backend (`OnevoHr.Api`)

### 2.1 New entity: `AgentDevice`

| Field | Type | Notes |
|---|---|---|
| `Id` | Guid | PK |
| `TenantId` | Guid? | null until `Confirmed` |
| `EmployeeId` | Guid? | null until `Confirmed` |
| `DeviceName` | string | machine name sent by the tray app at `POST /pair` |
| `UserCodeHash` | string | SHA-256 of the human-visible code (e.g. `ONEVO-JP82`); never store raw |
| `DeviceCodeHash` | string | SHA-256 of the long opaque polling secret; never store raw, never shown to the user |
| `Status` | enum | `PendingConfirmation`, `Confirmed`, `Enrolled`, `Expired`, `Cancelled` |
| `CreatedAt` | DateTimeOffset | |
| `ExpiresAt` | DateTimeOffset | `CreatedAt + 10 minutes` |
| `ConfirmedAt` | DateTimeOffset? | |
| `ConsentAcceptedAt` | DateTimeOffset? | |
| `DeviceTokenHash` | string? | SHA-256 of the long-lived API token issued at `Enrolled`; used to authenticate future desktop→backend calls |
| `FailedConfirmAttempts` | int | for rate-limiting |
| `LastSeenAt` | DateTimeOffset? | updated on each poll |

New EF Core migration under `backend/Data/Migrations`, following existing migration conventions in that folder.

### 2.2 New `DevicesController` (`api/v1/devices`)

| Endpoint | Auth | Purpose |
|---|---|---|
| `POST /pair` | anonymous (platform-level, no `X-Tenant-Domain`) | Body: `{ deviceName }`. Returns `{ userCode, deviceCode, expiresInSeconds, pollIntervalSeconds }`. |
| `GET /pair/status?deviceCode=...` | anonymous, keyed by the private `deviceCode` | Polled by the tray app. Returns `{ status }`, and once `Status = Confirmed`, also `{ employeeName, tenantName }` so the tray app can show who confirmed before presenting the consent screen. Never returns `deviceToken` — that only ever comes back once, from the `/consent` response. |
| `GET /pair/{userCode}` | requires existing session cookie auth + tenant context | Returns device info for the confirm page: device name, requested-at time, requesting tenant is implicit (the caller's own tenant). |
| `POST /pair/{userCode}/confirm` | requires session auth | Binds `TenantId` + `EmployeeId` from `ICurrentUserService`/`ITenantContextService` (never from request body). Sets `Status = Confirmed`. |
| `POST /pair/{userCode}/decline` | requires session auth | Browser-side Cancel. Sets `Status = Cancelled`. |
| `POST /pair/{deviceCode}/consent` | anonymous, keyed by the private `deviceCode` (proof of possession) | Called by the tray app itself after it observes `Status = Confirmed`. Sets `ConsentAcceptedAt`, `Status = Enrolled`, generates and returns a new opaque `deviceToken` (shown once, only its hash persisted). |
| `POST /pair/{deviceCode}/cancel` | anonymous, keyed by `deviceCode` | Tray-initiated cancel (e.g. user closes the pairing window). |

### 2.3 Security

- `userCode` is short and human-typeable, so:
  - 10-minute expiry, enforced server-side on every read.
  - `FailedConfirmAttempts` rate-limits `/confirm` — lock the code after 5 failed attempts (mismatched/expired) rather than allowing unlimited guesses.
  - `/pair/{userCode}` and `/confirm` require the caller to already be authenticated in that tenant — an unauthenticated party cannot use the code even if guessed.
- `deviceCode` is long/opaque (e.g. 32+ random bytes, base64url) and never shown to the user or logged — it is the tray app's private secret, functionally equivalent to a client credential.
- All codes/tokens stored as SHA-256 hashes only, matching how the existing session model already avoids storing raw secrets.

### 2.4 Code style

Follows `backend/CLAUDE.md`: explicit block-bodied methods (no expression-bodied `=>` on service/controller/repository logic), explicit DTO constructors, `IDevicePairingService`/`DevicePairingService` in the existing `Services/Interfaces` + `Services/Implementations` split, repository access via a new `IAgentDeviceRepository` following the existing repository pattern.

---

## 3. Frontend (`onevo-tenant-app`, Angular)

- New guarded route `/devices/confirm`, reusing the existing `core/guards/auth.guard.ts`.
- New `features/devices/device-confirm.component.ts` — reads `code` query param, calls `GET /pair/{userCode}`, renders the "Connect this desktop?" card (Employee / Device / Signed in as, Confirm / Cancel), matching the mockup's step 5 right-hand panel.
- New `core/api/endpoints/devices-api.service.ts`, mirroring the structure of the existing `core/api/endpoints/auth-api.service.ts`.
- No changes to `core/auth/auth.service.ts` or the existing login flow — device confirmation is a new, separate authenticated page, not a login mechanism.

---

## 4. Desktop (`OneVoTray`)

### 4.1 `LoginWindowViewModel`

Currently an empty stub. Add:
- `Step` (enum, `ObservableProperty`): `SignIn → RequestingCode → AwaitingConfirmation → Consent → Enrolled`, plus `Error` / `Expired`.
- `UserCode`, `DeviceName`, `EmployeeName`, `TenantName` (populated once available).
- `IsConsentChecked` (bound to the Monitoring Notice checkbox).
- `ErrorMessage`.
- Commands (`RelayCommand`): `SignInCommand` (kicks off `POST /pair` and starts polling), `OpenBrowserCommand` (opens `{frontendBaseUrl}/devices/confirm?code={UserCode}` in the system browser), `CopyCodeCommand` (clipboard), `CancelCommand`, `AcceptConsentCommand`.

### 4.2 `LoginWindow.xaml`

Single `ContentPage` inside the `Window` (replacing the placeholder username/password fields currently there). Sections toggled via `IsVisible` bound to `Step`:
- `SignIn` — the initial state, a "Sign in" button (also reachable from the tray icon's context menu).
- `RequestingCode` / `AwaitingConfirmation` — "Connect this desktop" card: `UserCode` display, Open Browser / Copy Code buttons, "Waiting for browser confirmation..." status.
- `Consent` — Monitoring Notice card: static copy from the mockup, checkbox, Accept & Continue / Cancel.
- `Enrolled` — status card: Employee, Device, Consent: Accepted, Monitoring: Paused, Reason: Not clocked in (static placeholders for now — real values come from the out-of-scope monitoring spec).

### 4.3 New services

- `Services/OnevoApiClient.cs` — `HttpClient` wrapper for the `/api/v1/devices/*` endpoints above. Separate from `NamedPipeClient.cs`, which remains an empty stub for a possible future local Agent IPC channel — unrelated to this HTTP flow.
- `Services/CredentialStore.cs` — thin Win32 Credential Manager wrapper (`CredWrite` / `CredRead` P/Invoke) to persist the `deviceToken` securely on the local machine, rather than storing it as a plain file.

### 4.4 Configuration

`ONEVO.Agent.Shared/Constants.cs` (currently empty) gains `ApiBaseUrl` and `FrontendBaseUrl` constants pointing at the dev backend/frontend ports (`http://localhost:5xxx`, `http://localhost:4200`). Production overrides are out of scope for this spec.

### 4.5 Polling

Tray app polls `GET /pair/status?deviceCode=...` every `pollIntervalSeconds` (server-suggested, default 3s) while in `AwaitingConfirmation`. On network failure, retry with simple backoff rather than failing the window.

---

## 5. Error Handling & Edge Cases

| Case | Behavior |
|---|---|
| `userCode` expires (10 min) before confirmation | Tray shows "Code expired" + a restart button that calls `POST /pair` again. |
| User clicks Cancel in the browser confirm page | `POST /pair/{userCode}/decline` → `Status = Cancelled` → tray's next poll sees `Cancelled` and shows a friendly message instead of timing out silently. |
| User closes/cancels the tray pairing window | `POST /pair/{deviceCode}/cancel` → `Status = Cancelled`, stops polling. |
| User declines the Monitoring Notice consent | Stays in `Confirmed` (not `Enrolled`); tray offers Retry consent or Cancel (which cancels the whole pairing). |
| Repeated failed confirm attempts on one `userCode` | Server-side lock after 5 attempts (`FailedConfirmAttempts`), independent of expiry. |
| Poll request fails (network blip) | Retry with backoff; do not crash or reset `Step`. |

---

## 6. Testing

- **Backend:** unit tests for `DevicePairingService` (state transitions, expiry, hashing, rate-limit lockout) and integration tests for `DevicesController` (following the existing `backend.Tests/Auth/AuthSessionTests.cs` / `CsrfTests.cs` patterns).
- **Frontend:** component test for `device-confirm.component.ts`.
- **Desktop:** manual verification (no existing automated test project for `OneVoTray`); `LoginWindowViewModel` state-transition logic is written so it could get unit tests later if a test project is added.

---

## 7. Key Decisions Log

- Device code is issued **tenant-agnostic**; tenant/employee binding happens at confirm time via the already-authenticated browser session — the tray app never asks the user for a company domain.
- A dedicated **device token** is minted at `Enrolled` for ongoing desktop→backend API auth, rather than the tray app trying to reuse/share the browser's session cookie.
- All pairing/consent/enrollment UI lives in a **single `LoginWindow`** with an internal step state machine, not separate popup windows per step.
- HRMS onboarding banner, installer packaging, and post-enrollment monitoring/clock-in features are explicitly deferred to future specs.
