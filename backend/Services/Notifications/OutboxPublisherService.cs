namespace OnevoHr.Api.Services.Notifications;

/// <summary>
/// Background worker that periodically publishes pending outbox messages.
/// In this learning backend "publishing" just marks messages as published.
/// </summary>
public class OutboxPublisherService : BackgroundService
{
    private static readonly TimeSpan PollInterval = TimeSpan.FromSeconds(30);

    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<OutboxPublisherService> _logger;

    public OutboxPublisherService(IServiceScopeFactory scopeFactory, ILogger<OutboxPublisherService> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();
                var outbox = scope.ServiceProvider.GetRequiredService<IOutboxService>();

                var pending = await outbox.GetPendingAsync(20);
                foreach (var message in pending)
                {
                    _logger.LogInformation("Publishing outbox message {MessageId} ({Type})", message.Id, message.Type);
                    await outbox.MarkPublishedAsync(message);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Outbox publishing failed");
            }

            try
            {
                await Task.Delay(PollInterval, stoppingToken);
            }
            catch (TaskCanceledException)
            {
                break;
            }
        }
    }
}
