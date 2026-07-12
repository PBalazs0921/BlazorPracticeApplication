using AutoMapper;
using BlazorApp1.Data;
using BlazorApp1.Entities.Dto;
using BlazorApp1.Entities.Entity;
using BlazorApp1.Logic.Dto;
using BlazorApp1.Logic.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace BlazorApp1.Logic;

public class ItemLogic(
    Repository<Item> repository,
    IUnitOfWork uow,
    IMemoryCache cache,
    DtoProvider dtoProvider):IItemLogic
{
    private const string AllItemsCacheKey = "items_all";

    // Shorter than other lists because ItemViewDto embeds a Category snapshot.
    // A category rename only evicts "categories_all", so this list can serve a
    // stale category name until it expires — bounded here to a few seconds.
    private static readonly TimeSpan CacheDuration = TimeSpan.FromSeconds(10);

    private readonly Mapper _mapper = dtoProvider.Mapper;

    public async Task<int?> CreateItemAsync(ItemCreateDto dto)
    {
        var item = _mapper.Map<Item>(dto);
        uow.Create(item);
        await uow.SaveChangesAsync();
        cache.Remove(AllItemsCacheKey);
        return item.Id;
    }

    public async Task<IEnumerable<ItemViewDto>> GetAllItemsAsync()
    {
        return await cache.GetOrCreateAsync(AllItemsCacheKey, async entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = CacheDuration;
            var allItems = await repository.GetAll()
                .Include(x => x.Category)
                .ToListAsync();
            return allItems.Select(x => _mapper.Map<ItemViewDto>(x)).ToList();
        }) ?? [];
    }
    public async Task<bool> UpdateItemAsync(ItemUpdateDto dto)
    {
        var itemToUpdate = await uow.Any<Item>()
            .FirstOrDefaultAsync(x => x.Id == dto.Id);
        if (itemToUpdate == null) return false;

        _mapper.Map(dto, itemToUpdate);
        await uow.SaveChangesAsync();
        cache.Remove(AllItemsCacheKey);
        return true;
    }

    public async Task<bool> DeleteItemAsync(int id)
    {
        var item = await uow.Any<Item>()
            .FirstOrDefaultAsync(x => x.Id == id);
        if (item == null) return false;

        uow.Remove(item);
        await uow.SaveChangesAsync();
        cache.Remove(AllItemsCacheKey);
        return true;
    }
}