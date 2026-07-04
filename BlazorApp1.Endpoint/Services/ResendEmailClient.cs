using System.Net;
using System.Net.Http.Json;
using System.Text.Json.Serialization;

namespace BlazorApp1.Endpoint.Services;

public sealed class ResendEmailClient(HttpClient httpClient)
{
    public async Task SendContactFormEmailAsync(string name, string email, string message, CancellationToken ct)
    {
        // User input goes into HTML, so encode it to prevent injection
        var safeName = WebUtility.HtmlEncode(name);
        var safeEmail = WebUtility.HtmlEncode(email);
        var safeMessage = WebUtility.HtmlEncode(message);

        var request = new ResendEmailRequest
        {
            From = "Portfolio Contact <onboarding@resend.dev>",
            To = ["peter.balazs0921@gmail.com"],
            Subject = $"New contact form message from {name}",
            Html = $"<p><strong>From:</strong> {safeName} ({safeEmail})</p><p>{safeMessage}</p>",
            ReplyTo = email
        };

        var response = await httpClient.PostAsJsonAsync("emails", request, ct);

        if (!response.IsSuccessStatusCode)
            throw new EmailSendException(response.StatusCode, await response.Content.ReadAsStringAsync(ct));
    }
}

file class ResendEmailRequest
{
    [JsonPropertyName("from")] public string From { get; set; } = "";
    [JsonPropertyName("to")] public List<string> To { get; set; } = [];
    [JsonPropertyName("subject")] public string Subject { get; set; } = "";
    [JsonPropertyName("html")] public string Html { get; set; } = "";
    [JsonPropertyName("reply_to")] public string? ReplyTo { get; set; }
}

public sealed class EmailSendException(HttpStatusCode statusCode, string message) : Exception(message)
{
    public HttpStatusCode StatusCode { get; } = statusCode;
}