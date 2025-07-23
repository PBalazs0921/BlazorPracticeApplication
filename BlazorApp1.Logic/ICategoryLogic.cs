using BlazorApp1.Entities.Dto;

namespace BlazorApp1.Logic;

public interface ICategoryLogic
{
    Task<IEnumerable<CategoryViewDto>> GetAllItemsAsync();
    Task<bool> UpdateItemAsync(CategoryUpdateDto dto);
    Task<CategoryViewDto?> CreateItemAsync(CategoryCreateDto dto);
    Task<bool> DeleteCategoryAsync(int id);
}