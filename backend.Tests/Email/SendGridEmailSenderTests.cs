using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using OnevoHr.Api.Services.Implementations;
using OnevoHr.Api.Services.Interfaces;
using Xunit;

namespace backend.Tests.Email;

/// <summary>
/// Unit tests for SendGridEmailSender using a stubbed HttpMessageHandler.
/// No real network calls and no real API keys — the fake key below is a
/// test-only placeholder. The channel config (including the API key) is
/// passed per send; the sender reads nothing from appsettings.
/// </summary>
public class SendGridEmailSenderTests
{
    private const string FakeApiKey = "test-key-not-real";

    private sealed class StubHttpHandler : HttpMessageHandler
    {
        private readonly HttpResponseMessage _response;
        public HttpRequestMessage? LastRequest { get; private set; }
        public string? LastRequestBody { get; private set; }

        public StubHttpHandler(HttpResponseMessage response)
        {
            _response = response;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            LastRequest = request;
            if (request.Content != null)
            {
                LastRequestBody = await request.Content.ReadAsStringAsync(cancellationToken);
            }
            return _response;
        }
    }

    private static SendGridEmailSender BuildSender(StubHttpHandler handler)
    {
        return new SendGridEmailSender(
            new HttpClient(handler),
            NullLogger<SendGridEmailSender>.Instance);
    }

    private static EmailChannelConfig BuildChannelConfig(string apiKey = FakeApiKey)
    {
        return new EmailChannelConfig(
            apiKey,
            "from@example.test",
            "OneVo",
            "reply@example.test");
    }

    private static SendEmailMessage BuildMessage()
    {
        return new SendEmailMessage(
            "recipient@example.test",
            "Test subject",
            "<p>Hello</p>",
            "Hello");
    }

    [Fact]
    public async Task SendAsync_Http202_ReturnsSuccess_WithMessageIdHeader()
    {
        var response = new HttpResponseMessage(HttpStatusCode.Accepted);
        response.Headers.Add("X-Message-Id", "abc-123");
        var handler = new StubHttpHandler(response);
        var sender = BuildSender(handler);

        var result = await sender.SendAsync(BuildMessage(), BuildChannelConfig(), default);

        Assert.True(result.Success);
        Assert.Equal("abc-123", result.ProviderMessageId);
        Assert.Null(result.Error);

        // Request shape: bearer auth + both content parts + recipient
        Assert.NotNull(handler.LastRequest);
        Assert.Equal("Bearer", handler.LastRequest!.Headers.Authorization!.Scheme);
        Assert.Equal(FakeApiKey, handler.LastRequest.Headers.Authorization!.Parameter);
        Assert.Contains("recipient@example.test", handler.LastRequestBody);
        Assert.Contains("from@example.test", handler.LastRequestBody);
        Assert.Contains("text/plain", handler.LastRequestBody);
        Assert.Contains("text/html", handler.LastRequestBody);
        Assert.Contains("reply_to", handler.LastRequestBody);
    }

    [Fact]
    public async Task SendAsync_RejectionResponse_ReturnsFailure_WithoutThrowing()
    {
        var response = new HttpResponseMessage(HttpStatusCode.BadRequest)
        {
            Content = new StringContent("{\"errors\":[{\"message\":\"bad from address\"}]}", Encoding.UTF8, "application/json")
        };
        var handler = new StubHttpHandler(response);
        var sender = BuildSender(handler);

        var result = await sender.SendAsync(BuildMessage(), BuildChannelConfig(), default);

        Assert.False(result.Success);
        Assert.Null(result.ProviderMessageId);
        Assert.Contains("400", result.Error);
        Assert.Contains("bad from address", result.Error);
        // The API key must never leak into the error text.
        Assert.DoesNotContain(FakeApiKey, result.Error);
    }

    [Fact]
    public async Task SendAsync_BlankApiKey_ReturnsFailure_WithoutCallingSendGrid()
    {
        var handler = new StubHttpHandler(new HttpResponseMessage(HttpStatusCode.Accepted));
        var sender = BuildSender(handler);

        var result = await sender.SendAsync(BuildMessage(), BuildChannelConfig(apiKey: ""), default);

        Assert.False(result.Success);
        Assert.Null(handler.LastRequest); // no HTTP call was made
    }

    [Fact]
    public async Task SendAsync_NullChannelConfig_ReturnsFailure_WithoutCallingSendGrid()
    {
        var handler = new StubHttpHandler(new HttpResponseMessage(HttpStatusCode.Accepted));
        var sender = BuildSender(handler);

        var result = await sender.SendAsync(BuildMessage(), null, default);

        Assert.False(result.Success);
        Assert.Null(handler.LastRequest); // no HTTP call was made
    }

    [Fact]
    public async Task SendAsync_NoReplyTo_OmitsReplyToBlock()
    {
        var handler = new StubHttpHandler(new HttpResponseMessage(HttpStatusCode.Accepted));
        var sender = BuildSender(handler);

        var config = new EmailChannelConfig(FakeApiKey, "from@example.test", "OneVo", null);
        var result = await sender.SendAsync(BuildMessage(), config, default);

        Assert.True(result.Success);
        Assert.DoesNotContain("reply_to", handler.LastRequestBody);
    }
}
