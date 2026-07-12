using AutoMapper;
using BlazorApp1.Data;
using BlazorApp1.Entities.Dto;
using BlazorApp1.Entities.Entity;
using BlazorApp1.Logic.Dto;
using BlazorApp1.Logic.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BlazorApp1.Logic;

public class CategoryLogic(
    Repository<Category> repository, 
    DtoProvider dtoProvider,
    IUnitOfWork uow) : ICategoryLogic
{
    private readonly Mapper _mapper = dtoProvider.Mapper;


    public async Task<CategoryViewDto?> CreateItemAsync(CategoryCreateDto dto)
    {   
        var category = _mapper.Map<Category>(dto);
        uow.Create(category);
        await uow.SaveChangesAsync();
        return _mapper.Map<CategoryViewDto>(category);
    }

    public async Task<IEnumerable<CategoryViewDto>> GetAllItemsAsync()
    {
        var allCategories = await repository.GetAllAsync();
        return allCategories.Select(x => _mapper.Map<CategoryViewDto>(x));
    }
    
    public async Task<bool> UpdateItemAsync(CategoryUpdateDto dto)
    {
        var categoryToUpdate = await uow.Any<Category>()
            .FirstOrDefaultAsync(x => x.Id == dto.Id);
        if (categoryToUpdate == null) return false;

        _mapper.Map(dto, categoryToUpdate);
        await uow.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteCategoryAsync(int id)
    {
        var item = await uow.Any<Category>()
            .FirstOrDefaultAsync(x => x.Id == id);
        if (item == null) return false;

        uow.Remove(item);
        await uow.SaveChangesAsync();
        return true;
    }
}