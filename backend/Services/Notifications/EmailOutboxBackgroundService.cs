using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OnevoHr.Api.Services.Interfaces;

namespace OnevoHr.Api.Services.Notifications;

/// <summary>
/// Background worker for the email_delivery_logs queue only. This is
/// separate from OutboxPublisherService, which only handles the generic
/// outbox_messages table — the two queues are never mixed.
///
/// Every poll cycle resolves IEmailOutboxProcessorService from a fresh scope
/// and asks it to send whatever is queued. Nothing here talks to
/// EmailDeliveryLogs or SendGrid directly; all of that logic already lives in
/// EmailOutboxProcessorService and must not be duplicated.
///
/// Polling is a simple hard-coded delay for this learning backend: 10
/// seconds in Development for fast feedback while testing invites locally,
/// 30 seconds otherwise. A status of "sent" here means SendGrid accepted the
/// message, not that it reached the recipient's inbox — real
/// delivered/bounced/complained states require SendGrid webhook support,
/// which does not exist yet.
/// </summary>
public class EmailOutboxBackgroundService : BackgroundService
{
    private static readonly TimeSpan DevelopmentPollInterval = TimeSpan.FromSeconds(10);
    private static readonly TimeSpan DefaultPollInterval = TimeSpan.FromSeconds(30);

    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<EmailOutboxBackgroundService> _logger;
    private readonly TimeSpan _pollInterval;

    public EmailOutboxBackgroundService(
        IServiceScopeFactory scopeFactory,
        ILogger<EmailOutboxBackgroundService> logger,
        IHostEnvironment environment)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
        _pollInterval = environment.IsDevelopment() ? DevelopmentPollInterval : DefaultPollInterval;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await ProcessOnceAsync(stoppingToken);

            try
            {
                await Task.Delay(_pollInterval, stoppingToken);
            }
            catch (TaskCanceledException)
            {
                break;
            }
        }
    }

    /// <summary>
    /// Runs a single poll: resolve the processor from a fresh scope, process
    /// whatever is queued, and swallow any exception so one bad cycle never
    /// permanently kills the worker. Internal (not private) so tests can
    /// exercise a single cycle without waiting on the real poll delay.
    /// </summary>
    internal async Task ProcessOnceAsync(CancellationToken ct)
    {
        try
        {
            using var scope = _scopeFactory.CreateScope();
            var processor = scope.ServiceProvider.GetRequiredService<IEmailOutboxProcessorService>();

            var processedCount = await processor.ProcessPendingEmailsAsync(ct);

            if (processedCount > 0)
            {
                _logger.LogInformation("Email outbox worker processed {ProcessedCount} email(s).", processedCount);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Email outbox worker failed while processing queued emails.");
        }
    }
}
