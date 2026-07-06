using System.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;

namespace OnevoHr.Api.Middleware;

/// <summary>
/// Middleware to log HTTP requests in Development environment with format:
/// METHOD PATH STATUS_CODE ELAPSED_MS
/// </summary>
public sealed class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;

    public RequestLoggingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            var stopwatch = Stopwatch.StartNew();
            try
            {
                await _next(context);
            }
            finally
            {
                stopwatch.Stop();
                var method = context.Request.Method;
                var path = context.Request.Path;
                var statusCode = context.Response.StatusCode;
                var elapsedMs = stopwatch.ElapsedMilliseconds;

                Console.WriteLine($"{method} {path} {statusCode} {elapsedMs}ms");
            }
        }
        else
        {
            await _next(context);
        }
    }
}
