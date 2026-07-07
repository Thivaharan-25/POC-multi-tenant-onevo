using System;
using System.ComponentModel.DataAnnotations;

namespace OnevoHr.Api.DTOs.Admin;

/// <summary>
/// Create/update a tenant's email provider channel. The apiKey is submitted
/// once, encrypted into notification_channels.credentials_encrypted, and is
/// never returned by any endpoint.
/// </summary>
public sealed class UpsertEmailChannelRequest
{
    [Required]
    public Guid TenantId { get; set; }

    [Required]
    public string Provider { get; set; } = "sendgrid";

    [Required]
    [EmailAddress]
    public string FromEmail { get; set; } = string.Empty;

    public string FromName { get; set; } = string.Empty;

    [EmailAddress]
    public string? ReplyToEmail { get; set; }

    /// <summary>
    /// Raw provider API key. Required when creating a channel; optional on
    /// update (omit to keep the existing stored credentials).
    /// </summary>
    public string? ApiKey { get; set; }
}

/// <summary>Safe channel metadata — never contains credentials.</summary>
public sealed record EmailChannelResponse(
    Guid Id,
    Guid TenantId,
    string Provider,
    string FromEmail,
    string FromName,
    string? ReplyToEmail,
    bool IsActive,
    bool HasCredentials);
