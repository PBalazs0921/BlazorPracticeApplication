namespace BlazorApp1.Entities.Dto;

public class UserCudDto
{
    public required string Email { get; set; } = "";

    public required string Password { get; set; } = "";

    public required string FamilyName { get; set; } = "";

    public required string GivenName { get; set; } = "";
}