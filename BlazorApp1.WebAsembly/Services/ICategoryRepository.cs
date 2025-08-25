using BlazorApp1.Entities.Dto;

namespace BlazorApp1.WebAsembly.Services;

public interface ICategoryRepository
{
    Task<List<CategoryViewDto>> GetCategories();
    Task DeleteItem(int id);
    Task AddCategory(CategoryCreateDto category);
    Task UpdateCategory(CategoryUpdateDto category);
}