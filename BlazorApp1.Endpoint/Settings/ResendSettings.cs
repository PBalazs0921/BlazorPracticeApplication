using System.ComponentModel.DataAnnotations;

namespace BlazorApp1.Endpoint.Settings;

public class ResendSettings
{
    public const string ConfigurationSection = "Resend";

    [Required]
    public string ApiKey { get; set; } = string.Empty;
}