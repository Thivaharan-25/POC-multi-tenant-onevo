using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using OnevoHr.Api.Models.Generated;
using OnevoHr.Api.Repositories.Interfaces;
using OnevoHr.Api.Services.Interfaces;

namespace OnevoHr.Api.Services.Implementations;

/// <summary>
/// Processes queued email_delivery_logs rows. For each row the tenant's
/// active SendGrid notification_channels row decides the route: when present,
/// the API key is decrypted in memory and the email goes through SendGrid;
/// when absent, the local_dev fallback marks the row dev_logged without any
/// network call. Each row is saved after every state change so a crash
/// mid-batch never loses progress or re-sends an already-sent email.
/// </summary>
public class EmailOutboxProcessorService : IEmailOutboxProcessorService
{
    private const int BatchSize = 50;
    private const string ProviderSendGrid = "sendgrid";
    private const string ProviderLocalDev = "local_dev";

    private readonly IEmailDeliveryLogRepository _emailDeliveryLogRepository;
    private readonly INotificationChannelRepository _notificationChannelRepository;
    private readonly ISecretProtector _secretProtector;
    private readonly ISendGridEmailSender _sendGridEmailSender;
    private readonly ILocalDevEmailSender _localDevEmailSender;
    private readonly ILogger<EmailOutboxProcessorService> _logger;

    public EmailOutboxProcessorService(
        IEmailDeliveryLogRepository emailDeliveryLogRepository,
        INotificationChannelRepository notificationChannelRepository,
        ISecretProtector secretProtector,
        ISendGridEmailSender sendGridEmailSender,
        ILocalDevEmailSender localDevEmailSender,
        ILogger<EmailOutboxProcessorService> logger)
    {
        _emailDeliveryLogRepository = emailDeliveryLogRepository;
        _notificationChannelRepository = notificationChannelRepository;
        _secretProtector = secretProtector;
        _sendGridEmailSender = sendGridEmailSender;
        _localDevEmailSender = localDevEmailSender;
        _logger = logger;
    }

    public async Task<int> ProcessPendingEmailsAsync(CancellationToken ct)
    {
        var queuedEmails = await _emailDeliveryLogRepository.GetQueuedEmailsAsync(BatchSize, ct);

        if (queuedEmails.Count == 0)
        {
            return 0;
        }

        var processedCount = 0;

        foreach (var email in queuedEmails)
        {
            // Guard: GetQueuedEmailsAsync only returns queued rows, but never
            // dispatch anything that is not still queued.
            if (email.Status != "queued")
            {
                continue;
            }

            // Mark the row as sending BEFORE dispatch so a crash between the
            // send and the final save is visible instead of silently re-queued.
            email.Status = "sending";
            email.AttemptCount++;
            email.UpdatedAt = DateTimeOffset.UtcNow;
            await _emailDeliveryLogRepository.SaveChangesAsync(ct);

            try
            {
                var message = new SendEmailMessage(
                    email.RecipientEmail,
                    email.SubjectSnapshot,
                    email.BodyHtmlSnapshot ?? string.Empty,
                    email.BodyTextSnapshot ?? string.Empty);

                var channel = await _notificationChannelRepository.GetActiveEmailChannelAsync(email.TenantId, ct);

                if (channel is null)
                {
                    // No active provider channel configured for this tenant:
                    // local_dev fallback, no network call, row becomes dev_logged.
                    var devResult = await _localDevEmailSender.SendAsync(message, null, ct);

                    email.Provider = ProviderLocalDev;
                    email.Status = "dev_logged";
                    email.ProviderMessageId = devResult.ProviderMessageId;
                    email.SentAt = DateTimeOffset.UtcNow;
                    email.LastError = null;
                    processedCount++;
                }
                else
                {
                    var channelConfig = BuildChannelConfig(channel);

                    email.Provider = ProviderSendGrid;
                    email.NotificationChannelId = channel.Id;

                    if (channelConfig is null)
                    {
                        // Bad ConfigJson or undecryptable credentials. The error
                        // text deliberately contains no secret material.
                        email.Status = "failed";
                        email.LastError = "The active SendGrid notification channel configuration is invalid.";
                        _logger.LogWarning(
                            "Email delivery {EmailId} failed: notification channel {ChannelId} has invalid configuration.",
                            email.Id,
                            channel.Id);
                    }
                    else
                    {
                        var result = await _sendGridEmailSender.SendAsync(message, channelConfig, ct);

                        if (result.Success)
                        {
                            email.Status = "sent";
                            email.ProviderMessageId = result.ProviderMessageId;
                            email.SentAt = DateTimeOffset.UtcNow;
                            email.LastError = null;
                            processedCount++;
                        }
                        else
                        {
                            email.Status = "failed";
                            email.LastError = result.Error;
                            _logger.LogWarning(
                                "Email delivery {EmailId} to {Recipient} failed: {Error}",
                                email.Id,
                                email.RecipientEmail,
                                result.Error);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error sending email {EmailId}.", email.Id);
                email.Status = "failed";
                email.LastError = ex.Message;
            }

            email.UpdatedAt = DateTimeOffset.UtcNow;
            await _emailDeliveryLogRepository.SaveChangesAsync(ct);
        }

        return processedCount;
    }

    /// <summary>
    /// Turns a notification_channels row into a per-send config: parse the
    /// non-secret ConfigJson metadata and decrypt the API key. Returns null on
    /// any parse/decrypt problem — the caller records a generic error so no
    /// secret or ciphertext detail ever reaches logs or the delivery row.
    /// </summary>
    private EmailChannelConfig? BuildChannelConfig(NotificationChannel channel)
    {
        string fromEmail;
        string fromName;
        string? replyToEmail;

        try
        {
            using var doc = JsonDocument.Parse(string.IsNullOrWhiteSpace(channel.ConfigJson) ? "{}" : channel.ConfigJson);
            var root = doc.RootElement;

            fromEmail = ReadStringProperty(root, "fromEmail") ?? string.Empty;
            fromName = ReadStringProperty(root, "fromName") ?? string.Empty;
            replyToEmail = ReadStringProperty(root, "replyToEmail");
        }
        catch (JsonException)
        {
            return null;
        }

        if (string.IsNullOrWhiteSpace(fromEmail))
        {
            return null;
        }

        string apiKey;
        try
        {
            apiKey = _secretProtector.Unprotect(channel.CredentialsEncrypted);
        }
        catch (Exception)
        {
            // Wrong key ring, tampered payload, or empty value — never log details.
            return null;
        }

        if (string.IsNullOrWhiteSpace(apiKey))
        {
            return null;
        }

        return new EmailChannelConfig(apiKey, fromEmail, fromName, replyToEmail);
    }

    private static string? ReadStringProperty(JsonElement root, string propertyName)
    {
        if (root.ValueKind == JsonValueKind.Object && root.TryGetProperty(propertyName, out var property) && property.ValueKind == JsonValueKind.String)
        {
            return property.GetString();
        }
        return null;
    }
}
