using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using BlazorApp1.Data.Helper;
using BlazorApp1.Entities.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace BlazorApp1.Endpoint.Controllers;

[ApiController]
[Route("[controller]")]
public class UserController : ControllerBase
{
    
    
    private UserManager<AppUser> _userManager;
    private RoleManager<IdentityRole> _roleManager;
    private IConfiguration _configuration;
    
    public UserController(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager, IConfiguration configuration)
    {
        this._configuration = configuration;
        this._userManager = userManager;
        this._roleManager = roleManager;
    }
    
    [HttpPost("register")]
    public async Task Register(UserCudDto dto)
    {
        var user = new AppUser
        {
            UserName = dto.Email,
            Email = dto.Email,
            EmailConfirmed = true,
            FamilyName = dto.FamilyName,
            GivenName = "",
            RefreshToken = "",
            RefreshTokenExpiryTime = DateTime.Now
        };
        var result = await _userManager.CreateAsync(user, dto.Password);
        
            if (_userManager.Users.Count() == 1)
        {
            await _roleManager.CreateAsync(new IdentityRole("Admin"));
            await _userManager.AddToRoleAsync(user, "Admin");
        }

        if (!result.Succeeded)
        {
            throw new Exception(string.Join("; ", result.Errors.Select(e => e.Description)));
        }
        
    }

    [HttpPost("login")]
    [ProducesResponseType(StatusCodes.Status200OK, Type=typeof(LoginResultDto))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] UserLoginDto dto)
    {
        var user = await _userManager.FindByEmailAsync(dto.Email);
        if (user == null)
        {
            throw new Exception("User not found");
        }

        var result = await _userManager.CheckPasswordAsync(user, dto.Password);
        if (result)
        {

            //todo: generate token
            var claim = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName!),
                new Claim(ClaimTypes.NameIdentifier, user.Id)
            };

            foreach (var role in await _userManager.GetRolesAsync(user))
            {
                claim.Add(new Claim(ClaimTypes.Role, role));
            }

            int expiryInMinutes = 24 * 60;
            int refreshTokenExpiryInMinutes = 24 * 60 * 7; // 7 days
            var token = GenerateAccessToken(claim, expiryInMinutes);
            var refreshToken = await GenerateRefreshToken(user);

            return Ok(new LoginResultDto()
            {
                AccessToken = new JwtSecurityTokenHandler().WriteToken(token),
                AccessTokenExpiration = DateTime.Now.AddMinutes(expiryInMinutes),
                RefreshToken = refreshToken,
                RefreshTokenExpiration = DateTime.Now.AddMinutes(refreshTokenExpiryInMinutes)
            });
        }
        else
        {
            return Unauthorized();
        }
    }
    
    [HttpGet]
    public IEnumerable<IdentityUser> GetAll()
    {
        return _userManager.Users.ToList();
    }

    public async Task<IActionResult> Refresh([FromBody] UserRefreshModel model)
    {
        var principal = GetPrincipalFromExpiredToken(model.AccessToken);

        if (principal?.Identity?.Name is null)
            return Unauthorized();
        var user = await _userManager.FindByNameAsync(principal.Identity.Name);

        if (user is null || user.RefreshToken != model.RefreshToken || user.RefreshTokenExpiryTime < DateTime.UtcNow)
            return Unauthorized();

        int expiryInMinutes = 24 * 60;
        var claim = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.UserName!),
            new Claim(ClaimTypes.NameIdentifier, user.Id)
        };
        var token = GenerateAccessToken(claim,expiryInMinutes);

        return Ok(new LoginResultDto
        {
            AccessToken = new JwtSecurityTokenHandler().WriteToken(token),
            AccessTokenExpiration = token.ValidTo,
            RefreshToken = model.RefreshToken
        });
    }
    
    private JwtSecurityToken GenerateAccessToken(IEnumerable<Claim>? claims, int expiryInMinutes)
    {
        var signinKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_configuration["jwt:key"] ?? throw new Exception("jwt:key not found in appsettings.json")));

        return new JwtSecurityToken(
            issuer: "movieclub.com",
            audience: "movieclub.com",
            claims: claims?.ToArray(),
            expires: DateTime.Now.AddMinutes(expiryInMinutes),
            signingCredentials: new SigningCredentials(signinKey, SecurityAlgorithms.HmacSha256)
        );
    }
    
    private async Task<string> GenerateRefreshToken(AppUser user)
    {
        var randomNumber = new byte[32];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(randomNumber);
            string result = Convert.ToBase64String(randomNumber);
            user.RefreshToken = result;
            await _userManager.UpdateAsync(user);
            return result;
        }
    }
    
    private ClaimsPrincipal? GetPrincipalFromExpiredToken (string token) 
    {
        var secret = _configuration["jwt:key"] ?? throw new InvalidOperationException("Secret not configured");

        var validation = new TokenValidationParameters
        {
            ValidIssuer = _configuration["JWT:ValidIssuer"],
            ValidAudience = _configuration["JWT:ValidAudience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret)),
            ValidateLifetime = false
        };

        return new JwtSecurityTokenHandler().ValidateToken(token, validation, out _);
    }


}
