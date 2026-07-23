using System.Net.Http.Json;
using BlazorApp1.Entities.Dto;

namespace BlazorApp1.WebAsembly.Services;

public class ItemRepository : IItemRepository
{
    private readonly HttpClient _httpClient;

    public ItemRepository(IHttpClientFactory factory)
    {
        _httpClient = factory.CreateClient("ServerApi");
    }

    public async Task<List<ItemViewDto>> GetItems()
    {
        var response = await _httpClient.GetAsync("Item/Get");
        if (response.IsSuccessStatusCode)
        {
            var items = await response.Content.ReadFromJsonAsync<List<ItemViewDto>>();
            return items ?? new List<ItemViewDto>();
        }

        throw new Exception("Failed to load items");
    }

    public async Task DeleteItem(int id)
    {
        var response = await _httpClient.DeleteAsync($"Item/Delete?id={id}");
        if (!response.IsSuccessStatusCode)
        {
            Console.WriteLine($"Failed to delete item with Id: {id}. Status Code: {response.StatusCode}");
        }
    }

    public async Task AddItem(ItemCreateDto item)
    {
        var response = await _httpClient.PostAsJsonAsync("Item/Create", item);
        if (!response.IsSuccessStatusCode)
        {
            Console.WriteLine($"Failed to add item. Status Code: {response.StatusCode}");
        }
    }

    public async Task UpdateItem(ItemUpdateDto item)
    {
        var response = await _httpClient.PutAsJsonAsync("Item/Edit", item);
        if (!response.IsSuccessStatusCode)
        {
            Console.WriteLine($"Failed to update item with Id: {item.Id}. Status Code: {response.StatusCode}");
        }
    }
}
