using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using OnevoHr.Api.Services.Interfaces;

namespace OnevoHr.Api.Services.Implementations;

/// <summary>
/// Fallback sender for local development and tests. Never sends real email —
/// it only logs the recipient/subject and reports success so queued
/// email_delivery_logs rows can move to dev_logged. Channel config is ignored.
/// </summary>
public class LocalDevEmailSender : ILocalDevEmailSender
{
    public const string ProviderMessageId = "local-dev";

    private readonly ILogger<LocalDevEmailSender> _logger;

    public LocalDevEmailSender(ILogger<LocalDevEmailSender> logger)
    {
        _logger = logger;
    }

    public Task<EmailSendResult> SendAsync(SendEmailMessage message, EmailChannelConfig? channelConfig, CancellationToken ct)
    {
        _logger.LogInformation(
            "LocalDev email sender: pretending to send '{Subject}' to {Recipient}.",
            message.Subject,
            message.ToEmail);

        var result = new EmailSendResult(true, ProviderMessageId, null);
        return Task.FromResult(result);
    }
}
