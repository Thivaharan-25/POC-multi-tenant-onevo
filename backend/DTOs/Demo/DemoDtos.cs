namespace OnevoHr.Api.DTOs.Demo;

public sealed record CreateDemoRequestDto(
    string CompanyName,
    string RequesterName,
    string RequesterEmail,
    string CountryCode,
    string RequestedCompanySizeRange);

public sealed record ApproveDemoRequestDto(
    Guid DemoProfileId,
    string TenantSlug,
    string Domain,
    string OwnerEmail,
    string OwnerPassword,
    string OwnerDisplayName);

public sealed record RejectDemoRequestDto(string Reason);

public sealed record DemoUpgradeQuoteRequestDto(Guid SubscriptionPlanId, string BillingCycle, int ConfirmedEmployeeCount);

public sealed record DemoUpgradeQuoteDto(
    Guid SubscriptionPlanId,
    string PlanName,
    string BillingCycle,
    int ConfirmedEmployeeCount,
    decimal Price,
    string Currency);

public sealed record DemoUpgradeSubmitRequestDto(
    Guid SubscriptionPlanId,
    string BillingCycle,
    int ConfirmedEmployeeCount,
    string CompanyLegalName,
    IReadOnlyCollection<string> SelectedFeatureKeys,
    IReadOnlyCollection<string> SelectedAddOnKeys);

public sealed record DemoUpgradeResultDto(Guid TenantId, string TenantStatus, Guid InvoiceId, string InvoiceNumber, decimal Amount, string InvoiceStatus);
