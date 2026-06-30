using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BlazorApp1.Endpoint.Controllers;

[ApiController]
[Route("[controller]")]
public class UserController : ControllerBase
{
    [HttpGet("me")]
    [Authorize]
    public IActionResult Me()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                     ?? User.FindFirst("sub")?.Value;
        var email = User.FindFirst(ClaimTypes.Email)?.Value
                    ?? User.FindFirst("email")?.Value;

        return Ok(new { userId, email });
    }
}
