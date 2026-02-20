using EmployeeAPI;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Net.Http.Json;
using Xunit;

public class AuthControllerIntegrationTests
    : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public AuthControllerIntegrationTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    // 🚀 This test now uses Theory + InlineData (xUnit approved)
    [Theory]
    [InlineData("aa", "bbb")]
    [InlineData("wrongUser", "wrongPass")]
    [InlineData("invalid", "invalid")]
    public async Task Login_InvalidCredentials_Returns401(string username, string password)
    {
        var response = await _client.PostAsJsonAsync(
            "/api/AuthController_EF/login",
            new { Username = username, Password = password }
        );

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}
