namespace OnevoHr.Api.Exceptions;

/// <summary>
/// Thrown when a caller tries to attach a permission that is outside the
/// tenant's current entitlements (module or feature disabled). Controllers
/// translate this into a 400 Bad Request, never a 500.
/// </summary>
public sealed class UnassignablePermissionException : Exception
{
    public UnassignablePermissionException(string message) : base(message)
    {
    }
}
