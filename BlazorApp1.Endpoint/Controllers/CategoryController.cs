using BlazorApp1.Entities.Dto;
using BlazorApp1.Entities.Helper;
using BlazorApp1.Logic;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BlazorApp1.Endpoint.Controllers;


[ApiController]
[Route("[controller]")]
public class CategoryController : ControllerBase
{
    
    private UserManager<AppUser> userManager;
    private CategoryLogic categoryLogic;
    
    public CategoryController(UserManager<AppUser> userManager, CategoryLogic categoryLogic)
    {
        this.userManager = userManager;
        this.categoryLogic = categoryLogic;
    }
    
    [HttpPost]
    public async Task Post([FromBody]CategoryCreateDto dto)
    {
        categoryLogic.CreateItem(dto);
    }
    
    [HttpGet]
    public async Task<IEnumerable<CategoryViewDto>> Get()
    {
        return categoryLogic.GetAllItems();
        
    }
    
    [HttpPut("Edit")]
    public async Task<IActionResult> Edit([FromBody] CategoryUpdateDto dto)
    {
        var success = categoryLogic.UpdateItem(dto);
    
        if (success)
            return Ok();
        else
            return NotFound("Category not found or update failed.");
    }
    
    [HttpPost("Create")]
    public async Task<IActionResult> Create([FromBody] CategoryCreateDto dto)
    {
        var result = categoryLogic.CreateItem(dto);

        if (result)
            return Ok();
        else
            return BadRequest("Failed to create category.");
    }

    
    [HttpDelete("Delete")]
    public async Task<bool> Delete([FromQuery] int id)
    {
        return categoryLogic.DeleteCategory(id);
    }

}