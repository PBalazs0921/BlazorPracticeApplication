using AutoMapper;
using BlazorApp1.Data;
using BlazorApp1.Entities.Dto;
using BlazorApp1.Entities.Entity;
using BlazorApp1.Logic.Dto;
using BlazorApp1.Logic.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace BlazorApp1.Logic;

public class CategoryLogic(
    Repository<Category> repository,
    IUnitOfWork uow,
    IMemoryCache cache,
    DtoProvider dtoProvider) : ICategoryLogic
{
    private const string AllCategoriesCacheKey = "categories_all";
    private static readonly TimeSpan CacheDuration = TimeSpan.FromSeconds(60);

    private readonly Mapper _mapper = dtoProvider.Mapper;


    public async Task<CategoryViewDto?> CreateItemAsync(CategoryCreateDto dto)
    {
        var category = _mapper.Map<Category>(dto);
        uow.Create(category);
        await uow.SaveChangesAsync();
        cache.Remove(AllCategoriesCacheKey);
        return _mapper.Map<CategoryViewDto>(category);
    }

    public async Task<IEnumerable<CategoryViewDto>> GetAllItemsAsync()
    {
        return await cache.GetOrCreateAsync(AllCategoriesCacheKey, async entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = CacheDuration;
            var allCategories = await repository.GetAllAsync();
            return allCategories.Select(x => _mapper.Map<CategoryViewDto>(x)).ToList();
        }) ?? [];
    }

    public async Task<bool> UpdateItemAsync(CategoryUpdateDto dto)
    {
        var categoryToUpdate = await uow.Any<Category>()
            .FirstOrDefaultAsync(x => x.Id == dto.Id);
        if (categoryToUpdate == null) return false;

        _mapper.Map(dto, categoryToUpdate);
        await uow.SaveChangesAsync();
        cache.Remove(AllCategoriesCacheKey);
        return true;
    }

    public async Task<bool> DeleteCategoryAsync(int id)
    {
        var item = await uow.Any<Category>()
            .FirstOrDefaultAsync(x => x.Id == id);
        if (item == null) return false;

        uow.Remove(item);
        await uow.SaveChangesAsync();
        cache.Remove(AllCategoriesCacheKey);
        return true;
    }
}
