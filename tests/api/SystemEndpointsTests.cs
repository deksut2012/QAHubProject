using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;

namespace QAHub.Api.Tests;

public sealed class SystemEndpointsTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public SystemEndpointsTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task HealthReturnsOk()
    {
        var response = await _client.GetAsync("/health");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task SystemInfoReturnsServiceIdentityAndCorrelationId()
    {
        const string correlationId = "integration-test-correlation";
        using var request = new HttpRequestMessage(HttpMethod.Get, "/api/v1/system/info");
        request.Headers.Add("X-Correlation-ID", correlationId);

        var response = await _client.SendAsync(request);
        var body = await response.Content.ReadFromJsonAsync<SystemInfoResponse>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("QAHub.Api", body?.Service);
        Assert.Equal("ready", body?.Status);
        Assert.Equal(correlationId, response.Headers.GetValues("X-Correlation-ID").Single());
    }

    [Fact]
    public async Task DevelopmentSessionUsesSystemAdministratorRole()
    {
        var response = await _client.GetAsync("/api/v1/session");
        var body = await response.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Contains("dev-user", body, StringComparison.Ordinal);
        Assert.Contains("SystemAdmin", body, StringComparison.Ordinal);
    }

    private sealed record SystemInfoResponse(string Service, string Version, string Status);
}
