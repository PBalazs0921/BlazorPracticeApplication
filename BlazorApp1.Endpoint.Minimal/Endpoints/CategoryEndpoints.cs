using BlazorApp1.Data;
using BlazorApp1.Entities.Dto;
using BlazorApp1.Entities.Entity;
using Microsoft.EntityFrameworkCore;

namespace BlazorApp1.Endpoint.Minimal.Endpoints;

public static class CategoryEndpoints
{
    public static async Task<IResult> GetAll(ApplicationDbContext repository)
    {
        return TypedResults.Ok(await repository.Categories.ToArrayAsync());
    }
    
    public static async Task<IResult> GetById(ApplicationDbContext repository, int id)
    {
        var result = await repository.Categories.FirstOrDefaultAsync(c=>c.Id==id);
        if (result == null)
            return TypedResults.NotFound();
        return TypedResults.Ok(result);
    }
}