namespace BlazorApp1.Entities.Dto;

public class UserLoginDto
{
    public required string Password { get; set; }
    
    public required string Email { get; set; }
}