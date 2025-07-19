using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using BlazorApp1;
using BlazorApp1.Entities.Dto;
using BlazorApp1.Entities.Helper;


namespace BlazorApp1.Controllers;

[ApiController]
[Route("[controller]")]
public class UserController : ControllerBase
{
    
    private UserManager<AppUser> userManager;
    private RoleManager<IdentityRole> roleManager;
    private IConfiguration configuration;
    
    public UserController(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager, IConfiguration configuration)
    {
        this.configuration = configuration;
        this.userManager = userManager;
        this.roleManager = roleManager;
    }
    
    [HttpPost("register")]
    public async Task Register(UserCUDDto dto)
    {
        var user = new AppUser
        {
            UserName = dto.Email,
            Email = dto.Email,
            EmailConfirmed = true,
            FamilyName = dto.FamilyName,
            GivenName = "",
            RefreshToken = ""
        };
        await userManager.CreateAsync(user, dto.Password);

        if (userManager.Users.Count() == 1)
        {
            await roleManager.CreateAsync(new IdentityRole("Admin"));
            await userManager.AddToRoleAsync(user, "Admin");
        }
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(UserLoginDto dto)
    {
        var user = await userManager.FindByEmailAsync(dto.Email);
        if (user == null)
        {
            throw new Exception("User not found");
        }

        var result = await userManager.CheckPasswordAsync(user, dto.Password);
        if (result)
        {

            //todo: generate token
            var claim = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName!),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
            };

            foreach (var role in await userManager.GetRolesAsync(user))
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
            throw new Exception("Invalid password");
        }
    }
    
    private async Task<string> GenerateRefreshToken(AppUser user)
    {
        var randomNumber = new byte[32];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(randomNumber);
            string result = Convert.ToBase64String(randomNumber);
            user.RefreshToken = result;
            await userManager.UpdateAsync(user);
            return result;
        }
    }

    [HttpGet]
    [Authorize]
    public IEnumerable<IdentityUser> Get()
    {
        return userManager.Users.ToList();
    }
    
    
    private JwtSecurityToken GenerateAccessToken(IEnumerable<Claim>? claims, int expiryInMinutes)
    {
        var signinKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(configuration["jwt:key"] ?? throw new Exception("jwt:key not found in appsettings.json")));

        return new JwtSecurityToken(
            issuer: "movieclub.com",
            audience: "movieclub.com",
            claims: claims?.ToArray(),
            expires: DateTime.Now.AddMinutes(expiryInMinutes),
            signingCredentials: new SigningCredentials(signinKey, SecurityAlgorithms.HmacSha256)
        );
    }
}
