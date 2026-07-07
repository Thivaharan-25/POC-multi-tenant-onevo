using Microsoft.AspNetCore.DataProtection;
using OnevoHr.Api.Services.Interfaces;

namespace OnevoHr.Api.Services.Implementations;

/// <summary>
/// ISecretProtector backed by ASP.NET Core Data Protection. The purpose
/// string is versioned so a future algorithm change can rotate to
/// "onevo.provider-credentials.v2" while still unprotecting v1 payloads.
/// </summary>
public class DataProtectionSecretProtector : ISecretProtector
{
    public const string ProtectorPurpose = "onevo.provider-credentials.v1";

    private readonly IDataProtector _protector;

    public DataProtectionSecretProtector(IDataProtectionProvider dataProtectionProvider)
    {
        _protector = dataProtectionProvider.CreateProtector(ProtectorPurpose);
    }

    public string Protect(string plaintext)
    {
        return _protector.Protect(plaintext);
    }

    public string Unprotect(string protectedValue)
    {
        return _protector.Unprotect(protectedValue);
    }
}
