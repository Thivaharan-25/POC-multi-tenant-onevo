namespace OnevoHr.Api.Options;

/// <summary>
/// Bound from the "Email" configuration section. Only non-secret settings live
/// here. SendGrid provider configuration (including the API key) is stored per
/// tenant in the notification_channels table — never in appsettings.
/// </summary>
public class EmailOptions
{
    public const string SectionName = "Email";

    /// <summary>Base URL used to build links in emails (e.g. the accept-invite URL).</summary>
    public string FrontendBaseUrl { get; set; } = "http://localhost:4200";
}
