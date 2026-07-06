namespace OnevoHr.Api.Services.Interfaces;

public interface IRlsBypassContext
{
    bool IsBypassEnabled { get; }
    IDisposable BeginTrustedRlsBypass();
}
