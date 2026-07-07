namespace OnevoHr.Api.Services.Interfaces;

/// <summary>
/// Encrypts and decrypts provider secrets (e.g. the SendGrid API key) stored
/// in notification_channels.credentials_encrypted. Implementations must never
/// log the plaintext or the protected value.
/// </summary>
public interface ISecretProtector
{
    string Protect(string plaintext);
    string Unprotect(string protectedValue);
}
