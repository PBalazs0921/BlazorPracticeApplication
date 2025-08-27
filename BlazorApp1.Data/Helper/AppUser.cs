using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace BlazorApp1.Data.Helper;
public class AppUser : IdentityUser
{
    [StringLength(200)]
    public required string FamilyName { get; set; } = "";

    [StringLength(200)]
    public required string GivenName { get; set; } = "";

    [StringLength(200)]
    public required string RefreshToken { get; set; } = "";

    public required DateTime RefreshTokenExpiryTime { get; set; } = DateTime.Now;
}