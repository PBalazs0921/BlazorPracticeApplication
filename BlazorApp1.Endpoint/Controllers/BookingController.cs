using BlazorApp1.Entities.Dto;
using BlazorApp1.Logic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlazorApp1.Endpoint.Controllers;

[ApiController]
[Route("[controller]")]
public class BookingController(BookingLogic bookingLogic) : ControllerBase
{
    [HttpGet("GetAll")]
    [Authorize]
    public async Task<IEnumerable<BookingViewDto>> GetAll()
    {
        return await bookingLogic.GetAllBookingsAsync();
    }

    [HttpPost("Create")]
    [Authorize]
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
    [Authorize]
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
    [Authorize]
    public async Task<IActionResult> Delete([FromQuery] int id)
    {
        var success = await bookingLogic.DeleteItemAsync(id);
        return success ? Ok() : NotFound("Booking not found.");
    }
}
