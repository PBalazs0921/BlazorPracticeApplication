using BlazorApp1.Entities.Dto;
using BlazorApp1.Entities.Helper;
using BlazorApp1.Logic;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BlazorApp1.Endpoint.Controllers;

[ApiController]
[Route("[controller]")]
public class BookingController(UserManager<AppUser> userManager, BookingLogic bookingLogic)
    : ControllerBase
{
    private readonly UserManager<AppUser> _userManager = userManager;

    [HttpGet("GetAll")]
    public async Task<IEnumerable<BookingViewDto>> GetAll()
    {
        return await bookingLogic.GetAllBookingsAsync();
    }

    [HttpPost("Create")]
    public async Task<IActionResult> Create([FromBody] BookingCreateDto dto)
    {
        try
        {
            var success = await bookingLogic.CreateBookingAsync(dto);
            return success ? Ok() : BadRequest("Failed to create booking.");
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("Update")]
    public async Task<IActionResult> Update([FromBody] BookingUpdateDto dto)
    {
        try
        {
            var success = await bookingLogic.UpdateBookingAsync(dto);
            return success ? Ok() : BadRequest("Failed to update booking.");
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpDelete("Delete")]
    public async Task<IActionResult> Delete([FromQuery] int id)
    {
        var success = await bookingLogic.DeleteItemAsync(id);
        if (success)
            return Ok();
        else
            return NotFound("Booking not found.");
    }
}