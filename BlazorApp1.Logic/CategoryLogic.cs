using AutoMapper;
using BlazorApp1.Data;
using BlazorApp1.Entities.Dto;
using BlazorApp1.Entities.Entity;
using BlazorApp1.Logic.Dto;

namespace BlazorApp1.Logic;

public class CategoryLogic(Repository<Category> repository, DtoProvider dtoProvider) : ICategoryLogic
{
    private readonly Mapper _mapper = dtoProvider.mapper;


    public async Task<CategoryViewDto> CreateItemAsync(CategoryCreateDto dto)
    {
        var category = _mapper.Map<Category>(dto);
        await repository.CreateAsync(category);
        return _mapper.Map<CategoryViewDto>(category);
    }

    public async Task<IEnumerable<CategoryViewDto>> GetAllItemsAsync()
    {
        var allCategories = await repository.GetAllAsync();
        return allCategories.Select(x => _mapper.Map<CategoryViewDto>(x));
    }
    
    public async Task<bool> UpdateItemAsync(CategoryUpdateDto dto)
    {
        var categoryToUpdate = await repository.FindByIdAsync(dto.Id);
        if (categoryToUpdate != null && categoryToUpdate.Id == dto.Id)
        {
            _mapper.Map(dto, categoryToUpdate);
            await repository.UpdateAsync(categoryToUpdate);
            return true;
        }

        return false;
    }
    
    public async Task<bool> DeleteCategoryAsync(int id)
    {
        Console.WriteLine("DeleteItem pressed: " + id);
    
        var item = await repository.FindByIdAsync(id);
        if (item == null) return false;

        await repository.DeleteAsync(item);
        return true;
    }
}