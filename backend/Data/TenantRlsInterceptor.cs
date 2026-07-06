using System.Data.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.Diagnostics;
using OnevoHr.Api.Services.Interfaces;
using OnevoHr.Api.Middleware;

namespace OnevoHr.Api.Data;

/// <summary>
/// EF Core connection interceptor that configures PostgreSQL Row-Level Security (RLS)
/// session variables ('app.current_tenant_id' and 'app.bypass_rls').
/// 
/// IMPORTANT FOR CONNECTION POOLING:
/// Session-scoped SET variables persist across connection returns. To prevent
/// cross-tenant data leakage when a connection is recycled from the EF Core pool,
/// we explicitly RESET these settings in ConnectionClosing/ConnectionClosingAsync.
/// </summary>
public sealed class TenantRlsInterceptor : DbConnectionInterceptor
{
    private readonly ITenantContextService _tenantContext;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IRlsBypassContext _rlsBypassContext;

    public TenantRlsInterceptor(
        ITenantContextService tenantContext,
        IHttpContextAccessor httpContextAccessor,
        IRlsBypassContext rlsBypassContext)
    {
        _tenantContext = tenantContext;
        _httpContextAccessor = httpContextAccessor;
        _rlsBypassContext = rlsBypassContext;
    }

    public override async Task ConnectionOpenedAsync(
        DbConnection connection,
        ConnectionEndEventData eventData,
        CancellationToken cancellationToken = default)
    {
        await SetTenantContextAsync(connection, cancellationToken);
    }

    public override void ConnectionOpened(
        DbConnection connection,
        ConnectionEndEventData eventData)
    {
        SetTenantContext(connection);
    }

    public override InterceptionResult ConnectionClosing(
        DbConnection connection,
        ConnectionEventData eventData,
        InterceptionResult result)
    {
        // Reset session settings to prevent tenant ID leakage when connection returns to the pool.
        using var cmd = connection.CreateCommand();
        cmd.CommandText = "RESET app.current_tenant_id; RESET app.bypass_rls;";
        cmd.ExecuteNonQuery();
        return result;
    }

    public override async ValueTask<InterceptionResult> ConnectionClosingAsync(
        DbConnection connection,
        ConnectionEventData eventData,
        InterceptionResult result)
    {
        // Reset session settings to prevent tenant ID leakage when connection returns to the pool.
        await using var cmd = connection.CreateCommand();
        cmd.CommandText = "RESET app.current_tenant_id; RESET app.bypass_rls;";
        await cmd.ExecuteNonQueryAsync();
        return result;
    }

    private void SetTenantContext(DbConnection connection)
    {
        var httpContext = _httpContextAccessor.HttpContext;
        using var cmd = connection.CreateCommand();

        string sql;
        if (_rlsBypassContext.IsBypassEnabled)
        {
            // Explicitly trusted bypass (e.g. database seeding, migrations, or other trusted scopes)
            sql = "SET app.bypass_rls = 'true';";
        }
        else if (httpContext != null && httpContext.Items.ContainsKey(CurrentUserMiddleware.PlatformSessionItemKey))
        {
            // Valid platform session validated by middleware: bypass RLS
            sql = "SET app.bypass_rls = 'true';";
        }
        else
        {
            // Deny/enforce by default
            var tenantId = _tenantContext.HasTenant && _tenantContext.TenantId.HasValue
                ? _tenantContext.TenantId.Value.ToString()
                : "00000000-0000-0000-0000-000000000000"; // Invalid ID forces deny-by-default if unresolved

            sql = $"SET app.current_tenant_id = '{tenantId}'; SET app.bypass_rls = 'false';";
        }

        cmd.CommandText = sql;
        cmd.ExecuteNonQuery();
    }

    private async Task SetTenantContextAsync(DbConnection connection, CancellationToken cancellationToken)
    {
        var httpContext = _httpContextAccessor.HttpContext;
        await using var cmd = connection.CreateCommand();

        string sql;
        if (_rlsBypassContext.IsBypassEnabled)
        {
            // Explicitly trusted bypass (e.g. database seeding, migrations, or other trusted scopes)
            sql = "SET app.bypass_rls = 'true';";
        }
        else if (httpContext != null && httpContext.Items.ContainsKey(CurrentUserMiddleware.PlatformSessionItemKey))
        {
            // Valid platform session validated by middleware: bypass RLS
            sql = "SET app.bypass_rls = 'true';";
        }
        else
        {
            // Deny/enforce by default
            var tenantId = _tenantContext.HasTenant && _tenantContext.TenantId.HasValue
                ? _tenantContext.TenantId.Value.ToString()
                : "00000000-0000-0000-0000-000000000000"; // Invalid ID forces deny-by-default if unresolved

            sql = $"SET app.current_tenant_id = '{tenantId}'; SET app.bypass_rls = 'false';";
        }

        cmd.CommandText = sql;
        await cmd.ExecuteNonQueryAsync(cancellationToken);
    }
}
