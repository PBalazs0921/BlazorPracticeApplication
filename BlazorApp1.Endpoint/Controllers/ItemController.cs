using BlazorApp1.Data.Helper;
using BlazorApp1.Entities.Dto;
using BlazorApp1.Logic.Interfaces;
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
    public async Task<IActionResult> Create([FromBody] ItemCreateDto dto)
    {
        var result = await itemLogic.CreateItemAsync(dto);

        if (result!=null)
            return Ok();
        else
            return BadRequest("Failed to create item.");
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