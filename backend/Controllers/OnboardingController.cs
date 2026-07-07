using Microsoft.AspNetCore.Mvc;
using OnevoHr.Api.DTOs;
using OnevoHr.Api.Services.Interfaces;
using OnevoHr.Api.Filters;

namespace OnevoHr.Api.Controllers;

[ApiController]
[Route("api/v1/onboarding/drafts")]
public class OnboardingController : ControllerBase
{
    private readonly IOnboardingService _onboardingService;

    public OnboardingController(IOnboardingService onboardingService)
    {
        _onboardingService = onboardingService;
    }

    /// <summary>
    /// POST /api/v1/onboarding/drafts
    /// Save or update an onboarding draft. Never creates user, employee, invite,
    /// checklist tasks, email log, or outbox event — those happen only at send-invite.
    /// Sets status=draft, draft_reason=saved_manually.
    /// </summary>
    [HttpPost]
    [RequirePermission("employees:write")]
    public async Task<IActionResult> SaveDraft([FromBody] SaveOnboardingDraftRequest request, CancellationToken ct)
    {
        var result = await _onboardingService.SaveDraftAsync(request, ct);
        if (result.Error != null)
        {
            return BadRequest(new { error = result.Error });
        }
        return Ok(new { id = result.DraftId });
    }

    /// <summary>
    /// GET /api/v1/onboarding/drafts/{id}
    /// Resume a draft. Enforces tenant isolation — only returns drafts owned by the
    /// current tenant. Returns 404 if not found or owned by a different tenant.
    /// </summary>
    [HttpGet("{id:guid}")]
    [RequirePermission("employees:write")]
    public async Task<IActionResult> GetDraft(Guid id, CancellationToken ct)
    {
        var draft = await _onboardingService.GetDraftAsync(id, ct);
        if (draft == null) return NotFound();
        return Ok(draft);
    }

    /// <summary>
    /// GET /api/v1/onboarding/drafts/mine
    /// Returns only draft-status onboarding drafts started by the current session
    /// user in the current tenant. Tenant and user are resolved server-side from
    /// ICurrentUserService — never from query or body.
    /// </summary>
    [HttpGet("mine")]
    [RequirePermission("employees:write")]
    public async Task<IActionResult> GetMyDrafts(CancellationToken ct)
    {
        var drafts = await _onboardingService.GetMyDraftsAsync(ct);
        return Ok(drafts);
    }

    /// <summary>
    /// POST /api/v1/onboarding/drafts/{id}/checklist
    /// Store the HR-reviewed and edited task list (selected_template_id +
    /// edited_tasks_json). Sets last_saved_step=checklist_review.
    /// </summary>
    [HttpPost("{id}/checklist")]
    [RequirePermission("employees:write")]
    public async Task<IActionResult> UpdateChecklistDraft(Guid id, [FromBody] UpdateChecklistDraftRequest request, CancellationToken ct)
    {
        var result = await _onboardingService.UpdateChecklistDraftAsync(id, request, ct);
        if (result.Status == "not_found") return NotFound();
        if (result.Status == "invalid_state" || result.Status == "invalid")
            return BadRequest(new { error = result.Message });
        return Ok(result);
    }

    /// <summary>
    /// POST /api/v1/onboarding/drafts/{id}/validate
    /// Runs pre-invite validation per docs (legal entity, department/position tenant
    /// ownership, work email uniqueness, employee number uniqueness, schedule exists,
    /// template exists, edited_tasks_json valid, required tasks not removed).
    /// Also surfaces seat-availability and sensitive-position warnings so HR can
    /// act before clicking Send Invite. Does NOT create any downstream rows.
    /// Returns 404 when draft not found or belongs to another tenant.
    /// </summary>
    [HttpPost("{id}/validate")]
    [RequirePermission("employees:write")]
    public async Task<IActionResult> ValidateDraft(Guid id, CancellationToken ct)
    {
        var result = await _onboardingService.ValidateDraftAsync(id, ct);
        if (result == null) return NotFound();
        return Ok(result);
    }

    /// <summary>
    /// POST /api/v1/onboarding/drafts/{id}/request-seat
    /// Only callable when draft_reason = waiting_for_seat. Creates a tenant-wide
    /// notification (Billing Manager Inbox surrogate) and a durable
    /// SeatIncreaseRequested outbox event. Returns 400 when the draft is in the
    /// wrong state. Returns 404 when draft belongs to another tenant or is missing.
    /// </summary>
    [HttpPost("{id}/request-seat")]
    [RequirePermission("employees:write")]
    public async Task<IActionResult> RequestSeat(Guid id, CancellationToken ct)
    {
        var result = await _onboardingService.RequestSeatAsync(id, ct);
        if (result.Status == "not_found") return NotFound();
        if (result.Status == "invalid_state") return BadRequest(new { error = result.Message });
        return Ok(result);
    }

    /// <summary>
    /// POST /api/v1/onboarding/drafts/{id}/submit-approval
    /// Only callable when draft_reason = waiting_for_position_approval. Creates a
    /// tenant-wide notification (Position Approver Inbox surrogate) and a durable
    /// PositionApprovalRequested outbox event. Returns 400 when in the wrong state.
    /// Returns 404 when draft belongs to another tenant or is missing.
    /// </summary>
    [HttpPost("{id}/submit-approval")]
    [RequirePermission("employees:write")]
    public async Task<IActionResult> SubmitPositionApproval(Guid id, CancellationToken ct)
    {
        var result = await _onboardingService.SubmitPositionApprovalAsync(id, ct);
        if (result.Status == "not_found") return NotFound();
        if (result.Status == "invalid_state") return BadRequest(new { error = result.Message });
        return Ok(result);
    }

    /// <summary>
    /// POST /api/v1/onboarding/drafts/{id}/send-invite
    /// Re-runs pre-invite validation, checks seat availability, checks sensitive
    /// position approval. When blocked: saves draft_reason and returns 200 with
    /// status=blocked (so frontend can show the correct follow-up action). When clear:
    /// creates employee, user (inactive/setup-required), invitation_tokens (hash only,
    /// no raw token), employee_lifecycle_events, employee_checklist_tasks,
    /// email_delivery_logs (status=queued), and the durable EmployeeOnboardingStarted
    /// outbox event — all in one SaveChangesAsync commit.
    /// </summary>
    [HttpPost("{id}/send-invite")]
    [RequirePermission("employees:write")]
    public async Task<IActionResult> SendInvite(Guid id, CancellationToken ct)
    {
        var result = await _onboardingService.SendInviteAsync(id, ct);

        if (result.Status == "not_found")
            return NotFound();

        // Validation errors: nothing was created. Return structured result so frontend
        // can display field-level errors, not a generic message.
        if (result.Status == "invalid")
            return BadRequest(new { status = result.Status, validation = result.Validation, error = result.Error });

        // Blocked (no seat or approval required): draft was updated but nothing else
        // was created. Return 200 so Angular can read draft_reason and show the right
        // action button (Request Seat or Submit Approval).
        if (result.Status == "blocked")
            return Ok(new { status = result.Status, draftReason = result.DraftReason, actions = result.Actions, validation = result.Validation });

        // Completed: all downstream rows created in one commit.
        return Ok(new
        {
            status = result.Status,
            dev_invite_url = result.DevInviteUrl,
            employeeId = result.EmployeeId,
            userId = result.UserId,
            validation = result.Validation
        });
    }
}
