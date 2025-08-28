using System.Net.Http.Json;
using BlazorApp1.Entities.Dto;
using Xunit;

namespace BlazorApp1.IntegrationTests.Tests.APITests;

[Collection("ApiTestCollection")]
public class AuthenticationTests
{
    private readonly HttpClient _client;

    public AuthenticationTests(IntegrationTestWebAppFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }
    
    [Fact]
    public async Task Login_ValidCredentials_ReturnsJwt()
    {
        // Arrange
        var loginDto = new UserLoginDto
        {
            Email = "admin@system.local",
            Password = "Admin@123"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/User/login", loginDto);
        var content = await response.Content.ReadFromJsonAsync<LoginResultDto>();
        // Assert
        Assert.True(response.IsSuccessStatusCode, $"Status: {response.StatusCode}, Body: {await response.Content.ReadAsStringAsync()}");
        Assert.NotNull(content);
        Assert.False(string.IsNullOrEmpty(content.AccessToken));
        Assert.False(string.IsNullOrEmpty(content.RefreshToken));
    }

    [Fact]
    public async Task Register_NewUser_ReturnsSuccess()
    {
        var NewUser = new UserCudDto()
        {
            Email = "String@gmail.com",
            Password = "String@1234",
        };
        var response = await _client.PostAsJsonAsync("/User/register", NewUser);
        var content = response.Content;
        Assert.True(response.IsSuccessStatusCode, $"Status: {response.StatusCode}, Body: {await response.Content.ReadAsStringAsync()}");
        Assert.NotNull(content);
    }
    
}