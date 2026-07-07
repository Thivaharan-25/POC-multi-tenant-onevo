using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using OnevoHr.Api.Data;
using OnevoHr.Api.Models.Generated;
using OnevoHr.Api.Repositories.Implementations;
using OnevoHr.Api.Services.Implementations;
using OnevoHr.Api.Services.Interfaces;
using OnevoHr.Api.Tests.TestInfrastructure;
using Xunit;
using Microsoft.EntityFrameworkCore;

namespace backend.Tests;

public class OutboxTests : IClassFixture<CustomWebApplicationFactory>
{
    private const string FakeApiKey = "SG.test-key-not-real";

    private readonly CustomWebApplicationFactory _factory;

    public OutboxTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }

    private static EmailDeliveryLog BuildQueuedEmail(string recipient, string provider, Guid? tenantId = null)
    {
        return new EmailDeliveryLog
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId ?? Guid.NewGuid(),
            RecipientEmail = recipient,
            SubjectSnapshot = "Test",
            BodyHtmlSnapshot = "<p>Test body</p>",
            BodyTextSnapshot = "Test body",
            Provider = provider,
            Status = "queued",
            AttemptCount = 0,
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.UtcNow
        };
    }

    private static NotificationChannel BuildActiveSendGridChannel(Guid tenantId, string credentialsEncrypted)
    {
        return new NotificationChannel
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            ChannelType = "email",
            Provider = "sendgrid",
            ConfigJson = "{\"fromEmail\":\"from@onevo.test\",\"fromName\":\"OneVo\",\"replyToEmail\":\"reply@onevo.test\"}",
            CredentialsEncrypted = credentialsEncrypted,
            IsActive = true,
            ConfiguredById = Guid.NewGuid(),
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.UtcNow
        };
    }

    /// <summary>
    /// Fake sender used to prove the processor's state transitions and that
    /// the per-send config comes from the DB channel. No API keys anywhere in
    /// tests — the fake key above is a placeholder.
    /// </summary>
    private sealed class FakeEmailSender : ISendGridEmailSender, ILocalDevEmailSender
    {
        private readonly EmailSendResult _result;
        public int SendCount { get; private set; }
        public EmailChannelConfig? LastChannelConfig { get; private set; }

        public FakeEmailSender(EmailSendResult result)
        {
            _result = result;
        }

        public Task<EmailSendResult> SendAsync(SendEmailMessage message, EmailChannelConfig? channelConfig, CancellationToken ct)
        {
            SendCount++;
            LastChannelConfig = channelConfig;
            return Task.FromResult(_result);
        }
    }

    private EmailOutboxProcessorService BuildProcessor(
        AppDbContext db,
        ISendGridEmailSender sendGridSender,
        ILocalDevEmailSender localDevSender)
    {
        var secretProtector = _factory.Services.GetRequiredService<ISecretProtector>();
        return new EmailOutboxProcessorService(
            new EmailDeliveryLogRepository(db),
            new NotificationChannelRepository(db),
            secretProtector,
            sendGridSender,
            localDevSender,
            NullLogger<EmailOutboxProcessorService>.Instance);
    }

    // ------------------------------------------------------------------
    // Local dev fallback: no active channel in the DB
    // ------------------------------------------------------------------

    [Fact]
    public async Task ProcessPendingEmails_NoActiveChannel_MarksDevLogged_WithoutSendGridCall()
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        // Tenant with NO notification_channels row at all
        var emailLog = BuildQueuedEmail($"outbox-{Guid.NewGuid():N}@acme.test", "local_dev");
        db.EmailDeliveryLogs.Add(emailLog);
        await db.SaveChangesAsync();

        var sendGridSender = new FakeEmailSender(new EmailSendResult(true, "sg-should-not-be-used", null));
        var localDevSender = new FakeEmailSender(new EmailSendResult(true, LocalDevEmailSender.ProviderMessageId, null));
        var processor = BuildProcessor(db, sendGridSender, localDevSender);

        var processedCount = await processor.ProcessPendingEmailsAsync(default);

        Assert.True(processedCount > 0);
        Assert.Equal(0, sendGridSender.SendCount); // no network path taken
        Assert.True(localDevSender.SendCount > 0);

        var updatedLog = await db.EmailDeliveryLogs.FindAsync(emailLog.Id);
        Assert.NotNull(updatedLog);
        Assert.Equal("dev_logged", updatedLog!.Status);
        Assert.Equal("local_dev", updatedLog.Provider);
        Assert.Equal(1, updatedLog.AttemptCount);
        Assert.NotNull(updatedLog.SentAt);
        Assert.Equal(LocalDevEmailSender.ProviderMessageId, updatedLog.ProviderMessageId);
    }

    [Fact]
    public async Task ProcessPendingEmails_InactiveChannel_FallsBackToDevLogged()
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var secretProtector = _factory.Services.GetRequiredService<ISecretProtector>();

        var tenantId = Guid.NewGuid();
        var inactiveChannel = BuildActiveSendGridChannel(tenantId, secretProtector.Protect(FakeApiKey));
        inactiveChannel.IsActive = false;
        db.NotificationChannels.Add(inactiveChannel);

        var emailLog = BuildQueuedEmail($"inactive-{Guid.NewGuid():N}@acme.test", "sendgrid", tenantId);
        db.EmailDeliveryLogs.Add(emailLog);
        await db.SaveChangesAsync();

        var sendGridSender = new FakeEmailSender(new EmailSendResult(true, "sg-should-not-be-used", null));
        var localDevSender = new FakeEmailSender(new EmailSendResult(true, LocalDevEmailSender.ProviderMessageId, null));
        var processor = BuildProcessor(db, sendGridSender, localDevSender);

        await processor.ProcessPendingEmailsAsync(default);

        Assert.Equal(0, sendGridSender.SendCount);
        var updatedLog = await db.EmailDeliveryLogs.FindAsync(emailLog.Id);
        Assert.Equal("dev_logged", updatedLog!.Status);
        Assert.Equal("local_dev", updatedLog.Provider);
    }

    // ------------------------------------------------------------------
    // SendGrid route: active channel loaded from the DB, key decrypted per send
    // ------------------------------------------------------------------

    [Fact]
    public async Task ProcessPendingEmails_ActiveChannel_UsesDbChannelConfig_MarksSent_StoresProviderMessageId()
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var secretProtector = _factory.Services.GetRequiredService<ISecretProtector>();

        var tenantId = Guid.NewGuid();
        var channel = BuildActiveSendGridChannel(tenantId, secretProtector.Protect(FakeApiKey));
        db.NotificationChannels.Add(channel);

        var emailLog = BuildQueuedEmail($"sent-{Guid.NewGuid():N}@acme.test", "sendgrid", tenantId);
        db.EmailDeliveryLogs.Add(emailLog);
        await db.SaveChangesAsync();

        var sendGridSender = new FakeEmailSender(new EmailSendResult(true, "sg-message-123", null));
        var localDevSender = new FakeEmailSender(new EmailSendResult(true, LocalDevEmailSender.ProviderMessageId, null));
        var processor = BuildProcessor(db, sendGridSender, localDevSender);

        var processedCount = await processor.ProcessPendingEmailsAsync(default);

        Assert.True(processedCount > 0);
        Assert.True(sendGridSender.SendCount > 0);
        Assert.Equal(0, localDevSender.SendCount);

        // The per-send config must come from the DB channel: decrypted key
        // plus ConfigJson metadata — nothing from appsettings.
        Assert.NotNull(sendGridSender.LastChannelConfig);
        Assert.Equal(FakeApiKey, sendGridSender.LastChannelConfig!.ApiKey);
        Assert.Equal("from@onevo.test", sendGridSender.LastChannelConfig.FromEmail);
        Assert.Equal("OneVo", sendGridSender.LastChannelConfig.FromName);
        Assert.Equal("reply@onevo.test", sendGridSender.LastChannelConfig.ReplyToEmail);

        var updatedLog = await db.EmailDeliveryLogs.FindAsync(emailLog.Id);
        Assert.NotNull(updatedLog);
        Assert.Equal("sent", updatedLog!.Status);
        Assert.Equal("sendgrid", updatedLog.Provider);
        Assert.Equal(channel.Id, updatedLog.NotificationChannelId);
        Assert.Equal("sg-message-123", updatedLog.ProviderMessageId);
        Assert.Equal(1, updatedLog.AttemptCount);
        Assert.NotNull(updatedLog.SentAt);
        Assert.Null(updatedLog.LastError);
    }

    [Fact]
    public async Task ProcessPendingEmails_SenderFailure_MarksFailed_And_StoresLastError()
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var secretProtector = _factory.Services.GetRequiredService<ISecretProtector>();

        var tenantId = Guid.NewGuid();
        db.NotificationChannels.Add(BuildActiveSendGridChannel(tenantId, secretProtector.Protect(FakeApiKey)));

        var emailLog = BuildQueuedEmail($"failed-{Guid.NewGuid():N}@acme.test", "sendgrid", tenantId);
        db.EmailDeliveryLogs.Add(emailLog);
        await db.SaveChangesAsync();

        var sendGridSender = new FakeEmailSender(new EmailSendResult(false, null, "SendGrid rejected the message with HTTP 400: bad request"));
        var localDevSender = new FakeEmailSender(new EmailSendResult(true, LocalDevEmailSender.ProviderMessageId, null));
        var processor = BuildProcessor(db, sendGridSender, localDevSender);

        await processor.ProcessPendingEmailsAsync(default);

        var updatedLog = await db.EmailDeliveryLogs.FindAsync(emailLog.Id);
        Assert.NotNull(updatedLog);
        Assert.Equal("failed", updatedLog!.Status);
        Assert.Contains("400", updatedLog.LastError);
        Assert.Equal(1, updatedLog.AttemptCount);
        Assert.Null(updatedLog.SentAt);
    }

    [Fact]
    public async Task ProcessPendingEmails_UndecryptableCredentials_MarksFailed_WithoutSecretInError()
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var tenantId = Guid.NewGuid();
        // Garbage ciphertext that ISecretProtector cannot unprotect
        db.NotificationChannels.Add(BuildActiveSendGridChannel(tenantId, "not-a-valid-protected-payload"));

        var emailLog = BuildQueuedEmail($"badkey-{Guid.NewGuid():N}@acme.test", "sendgrid", tenantId);
        db.EmailDeliveryLogs.Add(emailLog);
        await db.SaveChangesAsync();

        var sendGridSender = new FakeEmailSender(new EmailSendResult(true, "sg-should-not-be-used", null));
        var localDevSender = new FakeEmailSender(new EmailSendResult(true, LocalDevEmailSender.ProviderMessageId, null));
        var processor = BuildProcessor(db, sendGridSender, localDevSender);

        await processor.ProcessPendingEmailsAsync(default);

        Assert.Equal(0, sendGridSender.SendCount); // never dispatched with a bad key

        var updatedLog = await db.EmailDeliveryLogs.FindAsync(emailLog.Id);
        Assert.Equal("failed", updatedLog!.Status);
        Assert.NotNull(updatedLog.LastError);
        Assert.DoesNotContain("not-a-valid-protected-payload", updatedLog.LastError);
    }

    [Fact]
    public async Task ProcessPendingEmails_DoesNotTouch_NonQueued_Rows()
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var alreadySent = BuildQueuedEmail($"already-{Guid.NewGuid():N}@acme.test", "sendgrid");
        alreadySent.Status = "sent";
        alreadySent.AttemptCount = 1;
        db.EmailDeliveryLogs.Add(alreadySent);
        await db.SaveChangesAsync();

        var sendGridSender = new FakeEmailSender(new EmailSendResult(true, "sg-message-999", null));
        var localDevSender = new FakeEmailSender(new EmailSendResult(true, LocalDevEmailSender.ProviderMessageId, null));
        var processor = BuildProcessor(db, sendGridSender, localDevSender);

        await processor.ProcessPendingEmailsAsync(default);

        var untouched = await db.EmailDeliveryLogs.FindAsync(alreadySent.Id);
        Assert.NotNull(untouched);
        Assert.Equal("sent", untouched!.Status);
        Assert.Equal(1, untouched.AttemptCount);
    }

    // ------------------------------------------------------------------
    // Repository: the active channel really is loaded from the DB
    // ------------------------------------------------------------------

    [Fact]
    public async Task GetActiveEmailChannelAsync_Returns_Only_Active_SendGrid_Channel_For_Tenant()
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var secretProtector = _factory.Services.GetRequiredService<ISecretProtector>();

        var tenantId = Guid.NewGuid();
        var otherTenantId = Guid.NewGuid();

        var activeChannel = BuildActiveSendGridChannel(tenantId, secretProtector.Protect(FakeApiKey));
        var inactiveChannel = BuildActiveSendGridChannel(tenantId, secretProtector.Protect(FakeApiKey));
        inactiveChannel.IsActive = false;
        inactiveChannel.Provider = "sendgrid";
        var otherTenantChannel = BuildActiveSendGridChannel(otherTenantId, secretProtector.Protect(FakeApiKey));

        db.NotificationChannels.AddRange(activeChannel, inactiveChannel, otherTenantChannel);
        await db.SaveChangesAsync();

        var repository = new NotificationChannelRepository(db);

        var found = await repository.GetActiveEmailChannelAsync(tenantId, default);
        Assert.NotNull(found);
        Assert.Equal(activeChannel.Id, found!.Id);

        var missing = await repository.GetActiveEmailChannelAsync(Guid.NewGuid(), default);
        Assert.Null(missing);
    }
}
