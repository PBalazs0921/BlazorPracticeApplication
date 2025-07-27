using BlazorApp1.Entities.Dto;

namespace BlazorApp1.Logic.Interfaces;

public interface IItemLogic
{
    Task<IEnumerable<ItemViewDto>> GetAllItemsAsync();
    Task<bool> UpdateItemAsync(ItemUpdateDto dto);
    Task<int?> CreateItemAsync(ItemCreateDto dto);
    Task<bool> DeleteItemAsync(int id);
}