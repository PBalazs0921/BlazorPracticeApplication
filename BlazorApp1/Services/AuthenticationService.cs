using Blazored.SessionStorage;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Json;
using System.Reflection;
using System.Security.Claims;
using BlazorApp1.Entities.Dto;

namespace BlazorApp1.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IHttpClientFactory _factory;
        private ISessionStorageService _sessionStorageService;

        private const string JWT_KEY = nameof(JWT_KEY);
        private const string REFRESH_KEY = nameof(REFRESH_KEY);

        private string? _jwtCache;

        public event Action<string?>? LoginChange;

        public AuthenticationService(IHttpClientFactory factory, ISessionStorageService sessionStorageService)
        {
            _factory = factory;
            _sessionStorageService = sessionStorageService;
        }

        public async ValueTask<string> GetJwtAsync()
        {
            if (string.IsNullOrEmpty(_jwtCache))
                _jwtCache = await _sessionStorageService.GetItemAsync<string>(JWT_KEY);

            return _jwtCache;
        }

        public async Task LogoutAsync()
        {
            var response = await _factory.CreateClient("ServerApi").DeleteAsync("api/authentication/revoke");

            await _sessionStorageService.RemoveItemAsync(JWT_KEY);
            await _sessionStorageService.RemoveItemAsync(REFRESH_KEY);

            _jwtCache = null;

            await Console.Out.WriteLineAsync($"Revoke gave response {response.StatusCode}");

            LoginChange?.Invoke(null);
        }

        private static string GetUsername(string token)
        {
            var jwt = new JwtSecurityToken(token);

            return jwt.Claims.First(c => c.Type == ClaimTypes.Name).Value;
        }

        public async Task<DateTime> LoginAsync(UserLoginDto model)
        {
            var response = await _factory.CreateClient("MyAPI").PostAsync("/User/login",
                                                        JsonContent.Create(model));

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new UnauthorizedAccessException(
                    $"Login failed: {response.StatusCode} - {errorContent}");
            }

            var content = await response.Content.ReadFromJsonAsync<LoginResultDto>();

            if (content == null)
                throw new InvalidDataException();

            await _sessionStorageService.SetItemAsync(JWT_KEY, content.AccessToken);
            await _sessionStorageService.SetItemAsync(REFRESH_KEY, content.RefreshToken);

            LoginChange?.Invoke(GetUsername(content.AccessToken));

            return content.AccessTokenExpiration;
        }

    }
    
}