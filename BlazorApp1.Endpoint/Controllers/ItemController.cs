using BlazorApp1.Data.Helper;
using BlazorApp1.Entities.Dto;
using BlazorApp1.Logic.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BlazorApp1.Endpoint.Controllers;

[ApiController]
[Route("[controller]")]
public class ItemController(UserManager<AppUser> userManager, IItemLogic itemLogic) : ControllerBase
{
    private readonly UserManager<AppUser> _userManager = userManager;

    [HttpGet("Get")]
    public async Task<IEnumerable<ItemViewDto>> Get()
    {
        return await itemLogic.GetAllItemsAsync();
    }


    [HttpPut("Edit")]
    public async Task<IActionResult> Edit([FromBody] ItemUpdateDto dto)
    {
        var success = await itemLogic.UpdateItemAsync(dto);

        if (success)
            return Ok();
        else
            return NotFound("Item not found or update failed.");
    }

    [HttpPost("Create")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create([FromBody] ItemCreateDto dto)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user != null)
        {
            var result = await itemLogic.CreateItemAsync(dto);

            if (result!=null)
                return Ok();
            else
                return BadRequest("Failed to create item.");
        }else
        {
            // Handle the case when the user is not found
            // For example, return an error response or throw an exception
            throw new Exception("User not found");
        }

    }
    

    [HttpDelete("Delete")]
    public async Task<IActionResult> Delete([FromQuery] int id)
    {
        var result = await itemLogic.DeleteItemAsync(id);
        if (result)
            return Ok();
        else
            return NotFound("Item not found or delete failed.");
    }
}