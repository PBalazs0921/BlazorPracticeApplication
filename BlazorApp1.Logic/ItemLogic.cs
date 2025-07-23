using AutoMapper;
using BlazorApp1.Data;
using BlazorApp1.Entities.Dto;
using BlazorApp1.Entities.Entity;
using BlazorApp1.Logic.Dto;

namespace BlazorApp1.Logic;

public class ItemLogic(Repository<Item> repository, DtoProvider dtoProvider)
{
    private readonly Mapper _mapper = dtoProvider.mapper;

    public async Task<int?> CreateItemAsync(ItemCreateDto dto)
    {
        var item = _mapper.Map<Item>(dto);
        await repository.CreateAsync(item);
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
        var itemToUpdate = await repository.FindByIdAsync(dto.Id);
        if (itemToUpdate != null)
        {
            _mapper.Map(dto, itemToUpdate);
            await repository.UpdateAsync(itemToUpdate);
            return true;
        }
        return false;
    }

    public async Task<bool> DeleteItemAsync(int id)
    {
        Console.WriteLine("DeleteItem pressed: " + id);
    
        var item = await repository.FindByIdAsync(id);
        if (item == null) return false;

        await repository.DeleteAsync(item);
        return true;
    }
}