using System.Threading;
using System.Threading.Tasks;

namespace OnevoHr.Api.Services.Interfaces;

/// <summary>
/// Sends a single rendered email. Provider configuration (including the
/// decrypted API key) is passed per send — it comes from the tenant's
/// notification_channels row, never from appsettings, and is held in memory
/// only for the duration of the call.
/// </summary>
public interface IEmailSender
{
    Task<EmailSendResult> SendAsync(SendEmailMessage message, EmailChannelConfig? channelConfig, CancellationToken ct);
}

/// <summary>Real SendGrid dispatch. Requires a non-null channel config.</summary>
public interface ISendGridEmailSender : IEmailSender
{
}

/// <summary>No-op fallback for local development and tests. Ignores channel config.</summary>
public interface ILocalDevEmailSender : IEmailSender
{
}

public sealed record SendEmailMessage(
    string ToEmail,
    string Subject,
    string HtmlBody,
    string TextBody);

/// <summary>
/// Per-send provider configuration resolved from a notification_channels row:
/// ConfigJson metadata plus the decrypted API key. Never persist or log it.
/// </summary>
public sealed record EmailChannelConfig(
    string ApiKey,
    string FromEmail,
    string FromName,
    string? ReplyToEmail);

public sealed record EmailSendResult(
    bool Success,
    string? ProviderMessageId,
    string? Error);
