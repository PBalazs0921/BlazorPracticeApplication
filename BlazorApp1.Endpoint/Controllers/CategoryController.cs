using BlazorApp1.Entities.Dto;
using BlazorApp1.Logic;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BlazorApp1.Endpoint.Controllers;


[ApiController]
[Route("[controller]")]
public class CategoryController : ControllerBase
{
    
    private UserManager<IdentityUser> userManager;
    private CategoryLogic categoryLogic;
    
    public CategoryController(UserManager<IdentityUser> userManager, CategoryLogic categoryLogic)
    {
        this.userManager = userManager;
        this.categoryLogic = categoryLogic;
    }
    
    [HttpPost]
    public async Task Post([FromBody]CategoryCreateDto dto)
    {
    }
    
    [HttpGet]
    public async Task<IEnumerable<CategoryViewDto>> Get()
    {
        var user = await userManager.GetUserAsync(User);
        if (user != null)
        {
            return categoryLogic.GetAllItems();
        }
        else
        {
            // Handle the case when the user is not found
            // For example, return an error response or throw an exception
            throw new Exception("User not found");
        }
    }
}