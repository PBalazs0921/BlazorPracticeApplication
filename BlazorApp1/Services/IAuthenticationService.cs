using BlazorApp1.Entities.Dto;

namespace BlazorApp1.Services;

public interface IAuthenticationService
{
    event Action<string?>? LoginChange;
    ValueTask<string> GetJwtAsync();
    Task LogoutAsync();
    Task<DateTime> LoginAsync(UserLoginDto model);
}