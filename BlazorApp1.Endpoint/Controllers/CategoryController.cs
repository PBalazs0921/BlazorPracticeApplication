using BlazorApp1.Entities.Dto;
using BlazorApp1.Entities.Helper;
using BlazorApp1.Logic;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BlazorApp1.Endpoint.Controllers;


[ApiController]
[Route("[controller]")]
public class CategoryController(UserManager<AppUser> userManager, ICategoryLogic categoryLogic)
    : ControllerBase
{
    
    private UserManager<AppUser> _userManager = userManager;
    
    [HttpGet("GetAll")]
    public async Task<IEnumerable<CategoryViewDto>> GetAllAsync()
    {
        return await categoryLogic.GetAllItemsAsync();
        
    }
    
    [HttpPut("Edit")]
    public async Task<IActionResult> Edit([FromBody] CategoryUpdateDto dto)
    {
        var success = await categoryLogic.UpdateItemAsync(dto);
    
        if (success)
            return Ok();
        else
            return NotFound("Category not found or update failed.");
    }
    
    [HttpPost("Create")]
    public async Task<IActionResult> Create([FromBody] CategoryCreateDto dto)
    {
        var result = await categoryLogic.CreateItemAsync(dto);

        if (result.Id!= null)
            return Ok();
        else
            return BadRequest("Failed to create category.");
    }

    
    [HttpDelete("Delete")]
    public async Task<bool> Delete([FromQuery] int id)
    {
        var result = await categoryLogic.DeleteCategoryAsync(id);
        return result;
    }

}