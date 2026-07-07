using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using OnevoHr.Api.Services.Interfaces;

namespace OnevoHr.Api.Services.Implementations;

/// <summary>
/// Sends email through the SendGrid v3 mail/send HTTP API. Uses HttpClient
/// directly (no SendGrid package). HTTP 202 Accepted counts as success;
/// rejection responses are returned as EmailSendResult failures, not thrown.
/// The API key arrives per send inside EmailChannelConfig (decrypted from the
/// tenant's notification_channels row); it is never read from appsettings,
/// never stored on this instance, and never logged.
/// </summary>
public class SendGridEmailSender : ISendGridEmailSender
{
    private const string MailSendUrl = "https://api.sendgrid.com/v3/mail/send";
    private const int MaxErrorBodyLength = 500;

    private readonly HttpClient _httpClient;
    private readonly ILogger<SendGridEmailSender> _logger;

    public SendGridEmailSender(
        HttpClient httpClient,
        ILogger<SendGridEmailSender> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<EmailSendResult> SendAsync(SendEmailMessage message, EmailChannelConfig? channelConfig, CancellationToken ct)
    {
        if (channelConfig is null || string.IsNullOrWhiteSpace(channelConfig.ApiKey))
        {
            return new EmailSendResult(false, null, "SendGrid channel configuration is missing an API key.");
        }

        if (string.IsNullOrWhiteSpace(channelConfig.FromEmail))
        {
            return new EmailSendResult(false, null, "SendGrid channel configuration is missing a from email.");
        }

        object payload;
        if (string.IsNullOrWhiteSpace(channelConfig.ReplyToEmail))
        {
            payload = new
            {
                personalizations = new[]
                {
                    new { to = new[] { new { email = message.ToEmail } } }
                },
                from = new
                {
                    email = channelConfig.FromEmail,
                    name = channelConfig.FromName
                },
                subject = message.Subject,
                content = new[]
                {
                    new { type = "text/plain", value = message.TextBody },
                    new { type = "text/html", value = message.HtmlBody }
                }
            };
        }
        else
        {
            payload = new
            {
                personalizations = new[]
                {
                    new { to = new[] { new { email = message.ToEmail } } }
                },
                from = new
                {
                    email = channelConfig.FromEmail,
                    name = channelConfig.FromName
                },
                reply_to = new
                {
                    email = channelConfig.ReplyToEmail
                },
                subject = message.Subject,
                content = new[]
                {
                    new { type = "text/plain", value = message.TextBody },
                    new { type = "text/html", value = message.HtmlBody }
                }
            };
        }

        using var request = new HttpRequestMessage(HttpMethod.Post, MailSendUrl);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", channelConfig.ApiKey);
        request.Content = JsonContent.Create(payload);

        HttpResponseMessage response;
        try
        {
            response = await _httpClient.SendAsync(request, ct);
        }
        catch (HttpRequestException ex)
        {
            // Network-level failure: report it as a failed send so the delivery
            // log records the error instead of the whole processor run dying.
            _logger.LogError(ex, "SendGrid request failed for recipient {Recipient}.", message.ToEmail);
            return new EmailSendResult(false, null, $"SendGrid request failed: {ex.Message}");
        }

        using (response)
        {
            if (response.StatusCode == HttpStatusCode.Accepted)
            {
                var providerMessageId = ReadMessageIdHeader(response);
                return new EmailSendResult(true, providerMessageId, null);
            }

            var responseBody = await response.Content.ReadAsStringAsync(ct);
            if (responseBody.Length > MaxErrorBodyLength)
            {
                responseBody = responseBody.Substring(0, MaxErrorBodyLength);
            }

            var error = $"SendGrid rejected the message with HTTP {(int)response.StatusCode}: {responseBody}";
            _logger.LogWarning(
                "SendGrid rejected email to {Recipient} with HTTP {StatusCode}.",
                message.ToEmail,
                (int)response.StatusCode);
            return new EmailSendResult(false, null, error);
        }
    }

    private static string? ReadMessageIdHeader(HttpResponseMessage response)
    {
        if (response.Headers.TryGetValues("X-Message-Id", out var values))
        {
            return values.FirstOrDefault();
        }
        return null;
    }
}
