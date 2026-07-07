using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging.Abstractions;
using OnevoHr.Api.Services.Interfaces;
using OnevoHr.Api.Services.Notifications;
using Xunit;

namespace backend.Tests.Email;

/// <summary>
/// Unit tests for the email_delivery_logs background worker. These exercise
/// the single-poll method directly instead of the real BackgroundService
/// loop, since the loop's delay is a hard-coded 10s/30s and waiting that
/// long in a unit test is impractical.
/// </summary>
public class EmailOutboxBackgroundServiceTests
{
    private sealed class FakeHostEnvironment : IHostEnvironment
    {
        public string EnvironmentName { get; set; } = Environments.Production;
        public string ApplicationName { get; set; } = "backend.Tests";
        public string ContentRootPath { get; set; } = AppContext.BaseDirectory;
        public IFileProvider ContentRootFileProvider { get; set; } = new NullFileProvider();
    }

    private sealed class FakeEmailOutboxProcessorService : IEmailOutboxProcessorService
    {
        private readonly Func<CancellationToken, Task<int>> _behavior;
        public int CallCount { get; private set; }

        public FakeEmailOutboxProcessorService(Func<CancellationToken, Task<int>> behavior)
        {
            _behavior = behavior;
        }

        public Task<int> ProcessPendingEmailsAsync(CancellationToken ct)
        {
            CallCount++;
            return _behavior(ct);
        }
    }

    private static EmailOutboxBackgroundService BuildWorker(IEmailOutboxProcessorService processor)
    {
        var services = new ServiceCollection();
        services.AddScoped(_ => processor);
        var provider = services.BuildServiceProvider();

        return new EmailOutboxBackgroundService(
            provider.GetRequiredService<IServiceScopeFactory>(),
            NullLogger<EmailOutboxBackgroundService>.Instance,
            new FakeHostEnvironment());
    }

    [Fact]
    public async Task ProcessOnceAsync_CallsProcessPendingEmailsAsync_OnTheResolvedProcessor()
    {
        var fakeProcessor = new FakeEmailOutboxProcessorService(_ => Task.FromResult(2));
        var worker = BuildWorker(fakeProcessor);

        await worker.ProcessOnceAsync(default);

        Assert.Equal(1, fakeProcessor.CallCount);
    }

    [Fact]
    public async Task ProcessOnceAsync_WhenProcessorThrows_DoesNotPropagate()
    {
        var fakeProcessor = new FakeEmailOutboxProcessorService(_ => throw new InvalidOperationException("boom"));
        var worker = BuildWorker(fakeProcessor);

        var exception = await Record.ExceptionAsync(() => worker.ProcessOnceAsync(default));

        Assert.Null(exception);
        Assert.Equal(1, fakeProcessor.CallCount);
    }
}
