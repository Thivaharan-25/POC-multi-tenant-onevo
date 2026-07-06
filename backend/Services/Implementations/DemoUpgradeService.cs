using System.Text.Json;
using OnevoHr.Api.DTOs.Demo;
using OnevoHr.Api.Models.Subscriptions;
using OnevoHr.Api.Repositories.Interfaces;
using OnevoHr.Api.Services.Interfaces;
using OnevoHr.Api.Services.Notifications;

namespace OnevoHr.Api.Services.Implementations;

/// <summary>
/// Demo upgrade: trial tenant selects an allowed paid plan, confirms employee
/// count and billing cycle; backend creates the first invoice and the tenant
/// moves to pending_payment. Activation happens after the invoice is paid.
/// </summary>
public class DemoUpgradeService : IDemoUpgradeService
{
    private readonly ITenantRepository _tenants;
    private readonly IDemoProfileRepository _demoProfiles;
    private readonly ISubscriptionRepository _subscriptions;
    private readonly IOutboxService _outbox;

    public DemoUpgradeService(
        ITenantRepository tenants,
        IDemoProfileRepository demoProfiles,
        ISubscriptionRepository subscriptions,
        IOutboxService outbox)
    {
        _tenants = tenants;
        _demoProfiles = demoProfiles;
        _subscriptions = subscriptions;
        _outbox = outbox;
    }

    public async Task<DemoUpgradeQuoteDto?> GetQuoteAsync(Guid tenantId, DemoUpgradeQuoteRequestDto request)
    {
        var tenant = await _tenants.GetByIdAsync(tenantId);
        if (tenant is null || tenant.Status != "trial")
        {
            return null;
        }

        if (!await IsPlanAllowedAsync(tenant.DemoProfileId, request.SubscriptionPlanId))
        {
            return null;
        }

        var plan = await _subscriptions.GetPlanByIdAsync(request.SubscriptionPlanId);
        if (plan is null || !plan.IsActive)
        {
            return null;
        }

        var bracket = plan.PriceBrackets.FirstOrDefault(b => CompanySizeRangeContains(b.CompanySizeRange, request.ConfirmedEmployeeCount));
        if (bracket is null)
        {
            return null;
        }

        var price = request.BillingCycle == "annual" ? bracket.AnnualPrice : bracket.BasePlanMonthlyPrice;
        return new DemoUpgradeQuoteDto(
            plan.Id, plan.Name, request.BillingCycle, request.ConfirmedEmployeeCount, price, bracket.Currency);
    }

    public async Task<DemoUpgradeResultDto?> SubmitUpgradeAsync(Guid tenantId, DemoUpgradeSubmitRequestDto request)
    {
        var quote = await GetQuoteAsync(tenantId,
            new DemoUpgradeQuoteRequestDto(request.SubscriptionPlanId, request.BillingCycle, request.ConfirmedEmployeeCount));
        if (quote is null)
        {
            return null;
        }

        var tenant = await _tenants.GetByIdAsync(tenantId);
        if (tenant is null)
        {
            return null;
        }

        var now = DateTime.UtcNow;
        var subscription = new TenantSubscription
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            SubscriptionPlanId = request.SubscriptionPlanId,
            Status = "pending_payment",
            BillingCycle = request.BillingCycle,
            ConfirmedEmployeeCount = request.ConfirmedEmployeeCount,
            SelectedFeatureKeysJson = JsonSerializer.Serialize(request.SelectedFeatureKeys),
            SelectedAddOnsJson = JsonSerializer.Serialize(request.SelectedAddOnKeys),
            CreatedAtUtc = now
        };
        await _subscriptions.AddSubscriptionAsync(subscription);

        var invoice = new SubscriptionInvoice
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            TenantSubscriptionId = subscription.Id,
            InvoiceNumber = $"INV-{now:yyyyMMdd}-{Guid.NewGuid().ToString("N")[..8].ToUpperInvariant()}",
            Status = "open",
            Amount = quote.Price,
            Currency = quote.Currency,
            DueAtUtc = now.AddDays(14),
            CreatedAtUtc = now
        };
        await _subscriptions.AddInvoiceAsync(invoice);

        tenant.Status = "pending_payment";
        tenant.ConfirmedEmployeeCount = request.ConfirmedEmployeeCount;

        await _outbox.EnqueueAsync(tenantId, "demo_upgrade_submitted",
            $"{{\"tenantId\":\"{tenantId}\",\"invoiceId\":\"{invoice.Id}\"}}");

        await _subscriptions.SaveChangesAsync();

        return new DemoUpgradeResultDto(
            tenantId, tenant.Status, invoice.Id, invoice.InvoiceNumber, invoice.Amount, invoice.Status);
    }

    private async Task<bool> IsPlanAllowedAsync(Guid? demoProfileId, Guid subscriptionPlanId)
    {
        if (demoProfileId is null)
        {
            return false;
        }

        var profile = await _demoProfiles.GetByIdAsync(demoProfileId.Value);
        if (profile?.UpgradeOptions is null)
        {
            return false;
        }

        var allowedPlanIds = JsonSerializer.Deserialize<List<string>>(profile.UpgradeOptions.AllowedPlanIds)
            ?? new List<string>();
        return allowedPlanIds.Contains(subscriptionPlanId.ToString());
    }

    /// <summary>Parses a "min-max" or "min+" company_size_range label and checks whether it contains the given count.</summary>
    private static bool CompanySizeRangeContains(string companySizeRange, int employeeCount)
    {
        if (companySizeRange.EndsWith('+'))
        {
            var openMin = int.Parse(companySizeRange[..^1]);
            return employeeCount >= openMin;
        }

        var parts = companySizeRange.Split('-');
        if (parts.Length != 2 || !int.TryParse(parts[0], out var min) || !int.TryParse(parts[1], out var max))
        {
            return false;
        }

        return employeeCount >= min && employeeCount <= max;
    }
}
