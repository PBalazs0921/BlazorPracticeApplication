namespace BlazorApp1.Entities.Dto;

public class UserRefreshModel
{
    public required string AccessToken { get; set; }
    public required string RefreshToken { get; set; }
}