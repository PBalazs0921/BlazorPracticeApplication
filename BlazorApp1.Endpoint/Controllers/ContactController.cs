using BlazorApp1.Endpoint.Services;
using BlazorApp1.Entities.Dto;
using Microsoft.AspNetCore.Mvc;

namespace BlazorApp1.Endpoint.Controllers;

[ApiController]
[Route("[controller]")]
public class ContactController(ResendEmailClient emailClient) : ControllerBase
{
    [HttpPost("Send")]
    public async Task<IActionResult> Send([FromBody] ContactFormDto dto, CancellationToken ct)
    {
        try
        {
            await emailClient.SendContactFormEmailAsync(dto.Name!, dto.Email!, dto.Message!, ct);
            return Ok();
        }
        catch (EmailSendException ex)
        {
            return StatusCode(StatusCodes.Status502BadGateway, $"Email provider rejected the request: {ex.StatusCode}");
        }
    }
}
