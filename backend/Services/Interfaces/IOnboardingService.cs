using System;
using System.Threading;
using System.Threading.Tasks;
using OnevoHr.Api.DTOs;
using OnevoHr.Api.Models.Employees;

namespace OnevoHr.Api.Services.Interfaces;

public class SaveDraftResult
{
    public Guid? DraftId { get; set; }
    public ValidationIssue? Error { get; set; }
}

public interface IOnboardingService
{
    Task<SaveDraftResult> SaveDraftAsync(SaveOnboardingDraftRequest request, CancellationToken ct);
    Task<OnboardingDraft?> GetDraftAsync(Guid id, CancellationToken ct);
    Task<DraftActionResponse> UpdateChecklistDraftAsync(Guid id, UpdateChecklistDraftRequest request, CancellationToken ct);
    Task<DraftValidationResult?> ValidateDraftAsync(Guid id, CancellationToken ct);
    Task<DraftActionResponse> RequestSeatAsync(Guid id, CancellationToken ct);
    Task<DraftActionResponse> SubmitPositionApprovalAsync(Guid id, CancellationToken ct);
    Task<SendInviteResponse> SendInviteAsync(Guid id, CancellationToken ct);
}
