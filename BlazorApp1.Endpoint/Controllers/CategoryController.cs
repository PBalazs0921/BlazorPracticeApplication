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
    }
    
    [HttpGet]
    public async Task<IEnumerable<CategoryViewDto>> Get()
    {

            throw new Exception("User not found");
        
    }
}