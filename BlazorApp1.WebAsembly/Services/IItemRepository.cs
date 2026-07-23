using BlazorApp1.Entities.Dto;

namespace BlazorApp1.WebAsembly.Services;

public interface IItemRepository
{
    Task<List<ItemViewDto>> GetItems();
    Task DeleteItem(int id);
    Task AddItem(ItemCreateDto item);
    Task UpdateItem(ItemUpdateDto item);
}
