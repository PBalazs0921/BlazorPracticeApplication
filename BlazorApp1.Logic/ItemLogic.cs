using AutoMapper;
using BlazorApp1.Data;
using BlazorApp1.Entities.Dto;
using BlazorApp1.Entities.Entity;
using BlazorApp1.Logic.Dto;
using BlazorApp1.Logic.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BlazorApp1.Logic;

public class ItemLogic(
    Repository<Item> repository, 
    IUnitOfWork uow, 
    DtoProvider dtoProvider):IItemLogic
{
    private readonly Mapper _mapper = dtoProvider.Mapper;

    public async Task<int?> CreateItemAsync(ItemCreateDto dto)
    {
        var item = _mapper.Map<Item>(dto);
        uow.Create(item);
        await uow.SaveChangesAsync();
        return item.Id;
    }
    
    public async Task<IEnumerable<ItemViewDto>> GetAllItemsAsync()
    {
        Console.WriteLine("GetAllItemsAsync called");
        var allItems = await repository.GetAllAsync();
        return allItems.Select(x => _mapper.Map<ItemViewDto>(x));
    }
    public async Task<bool> UpdateItemAsync(ItemUpdateDto dto)
    {
        var itemToUpdate = await uow.Any<Item>()
            .FirstOrDefaultAsync(x => x.Id == dto.Id);
        if (itemToUpdate == null) return false;

        _mapper.Map(dto, itemToUpdate);
        await uow.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteItemAsync(int id)
    {
        var item = await uow.Any<Item>()
            .FirstOrDefaultAsync(x => x.Id == id);
        if (item == null) return false;

        uow.Remove(item);
        await uow.SaveChangesAsync();
        return true;
    }
}