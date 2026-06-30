using BlazorApp1.Entities.Dto;
using BlazorApp1.Logic.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlazorApp1.Endpoint.Controllers;

[ApiController]
[Route("[controller]")]
public class ItemController(IItemLogic itemLogic) : ControllerBase
{
    [HttpGet("Get")]
    public async Task<IEnumerable<ItemViewDto>> Get()
    {
        return await itemLogic.GetAllItemsAsync();
    }

    [HttpPut("Edit")]
    [Authorize]
    public async Task<IActionResult> Edit([FromBody] ItemUpdateDto dto)
    {
        var success = await itemLogic.UpdateItemAsync(dto);
        return success ? Ok() : NotFound("Item not found or update failed.");
    }

    [HttpPost("Create")]
    [Authorize]
    public async Task<IActionResult> Create([FromBody] ItemCreateDto dto)
    {
        var result = await itemLogic.CreateItemAsync(dto);
        return result != null ? Ok() : BadRequest("Failed to create item.");
    }

    [HttpDelete("Delete")]
    [Authorize]
    public async Task<IActionResult> Delete([FromQuery] int id)
    {
        var result = await itemLogic.DeleteItemAsync(id);
        return result ? Ok() : NotFound("Item not found.");
    }
}
