using BlazorApp1.Entities.Dto;
using BlazorApp1.Logic.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlazorApp1.Endpoint.Controllers;

[ApiController]
[Route("[controller]")]
public class CategoryController(ICategoryLogic categoryLogic) : ControllerBase
{
    [HttpGet("GetAll")]
    public async Task<IEnumerable<CategoryViewDto>> GetAllAsync()
    {
        return await categoryLogic.GetAllItemsAsync();
    }

    [HttpPut("Edit")]
    [Authorize]
    public async Task<IActionResult> Edit([FromBody] CategoryUpdateDto dto)
    {
        var success = await categoryLogic.UpdateItemAsync(dto);
        return success ? Ok() : NotFound("Category not found or update failed.");
    }

    [HttpPost("Create")]
    [Authorize]
    public async Task<IActionResult> Create([FromBody] CategoryCreateDto dto)
    {
        var result = await categoryLogic.CreateItemAsync(dto);
        return result?.Id != null ? Ok(result) : BadRequest("Failed to create category.");
    }

    [HttpDelete("Delete")]
    [Authorize]
    public async Task<IActionResult> Delete([FromQuery] int id)
    {
        var result = await categoryLogic.DeleteCategoryAsync(id);
        return result ? Ok() : NotFound("Category not found.");
    }
}
