using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Json;
using System.Security.Claims;
using BlazorApp1.Entities.Dto;
using Blazored.SessionStorage;

namespace BlazorApp1.WebAsembly.Services
{
    public class AuthenticationService(IHttpClientFactory factory, ISessionStorageService sessionStorageService)
        : IAuthenticationService
    {
        private const string JWT_KEY = nameof(JWT_KEY);
        private const string REFRESH_KEY = nameof(REFRESH_KEY);

        private string? _jwtCache;

        public event Action<string?>? LoginChange;

        public async ValueTask<string> GetJwtAsync()
        {
            if (string.IsNullOrEmpty(_jwtCache))
                _jwtCache = await sessionStorageService.GetItemAsync<string>(JWT_KEY);

            return _jwtCache;
        }

        public async Task LogoutAsync()
        {
            var response = await factory.CreateClient("ServerApi").DeleteAsync("api/authentication/revoke");

            await sessionStorageService.RemoveItemAsync(JWT_KEY);
            await sessionStorageService.RemoveItemAsync(REFRESH_KEY);

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
            var response = await factory.CreateClient("ServerApi").PostAsync("/User/login",
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

            await sessionStorageService.SetItemAsync(JWT_KEY, content.AccessToken);
            await sessionStorageService.SetItemAsync(REFRESH_KEY, content.RefreshToken);
            _jwtCache= content.AccessToken;
            
            LoginChange?.Invoke(GetUsername(content.AccessToken));

            return content.AccessTokenExpiration;
        }

        public async Task<bool> RefreshAsync()
        {
            var model = new UserRefreshModel()
            {
                AccessToken = sessionStorageService.GetItemAsync<string>(JWT_KEY).Result,
                RefreshToken = sessionStorageService.GetItemAsync<string>(REFRESH_KEY).Result
            };
            var response = await factory.CreateClient("ServerApi").PostAsync("/User/Refresh",
                JsonContent.Create(model));
            if (!response.IsSuccessStatusCode)
            {
                //LOGOUT
                await LogoutAsync();

                await sessionStorageService.RemoveItemAsync(JWT_KEY);
                await sessionStorageService.RemoveItemAsync(REFRESH_KEY);
                _jwtCache = null;
                
                await Console.Out.WriteLineAsync("Refresh failed, logging out: " + response.StatusCode);
                LoginChange?.Invoke(null);
                return false;
            }
            else
            {
                var content = await response.Content.ReadFromJsonAsync<LoginResultDto>();
                if (content == null)
                    throw new InvalidDataException();

                await sessionStorageService.SetItemAsync(JWT_KEY, content.AccessToken);
                await sessionStorageService.SetItemAsync(REFRESH_KEY, content.RefreshToken);
                _jwtCache= content.AccessToken;
                
                LoginChange?.Invoke(GetUsername(content.AccessToken));
                return true;
            }
            
        }
    }
    
}