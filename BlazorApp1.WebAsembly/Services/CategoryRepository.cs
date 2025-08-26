using System.Net.Http.Json;
using BlazorApp1.Entities.Dto;

namespace BlazorApp1.WebAsembly.Services;

public class CategoryRepository : ICategoryRepository
{
    private readonly HttpClient _httpClient;

    public CategoryRepository(IHttpClientFactory factory)
    {
        _httpClient = factory.CreateClient("ServerApi");
    }
    
    public async Task<List<CategoryViewDto>> GetCategories()
    {
        var response = await _httpClient.GetAsync("Category/GetAll");
        if (response.IsSuccessStatusCode)
        {
            var categories = await response.Content.ReadFromJsonAsync<List<CategoryViewDto>>();
            return categories ?? new List<CategoryViewDto>();
        }
        else
        {
            // Handle error response
            throw new Exception("Failed to load categories");
        }
    }

    public async Task DeleteItem(int id)
    {
        Console.WriteLine($"Delete button clicked for category with Id: {id}");

        var response = await _httpClient!.DeleteAsync($"Category/Delete?id={id}");
        if (response.IsSuccessStatusCode)
        {
            Console.WriteLine($"Category with Id: {id} deleted successfully.");
        }
        else
        {
            Console.WriteLine($"Failed to delete category with Id: {id}. Status Code: {response.StatusCode}");
        }
    }
    
    public async Task AddCategory(CategoryCreateDto category)
    {
        var response = await _httpClient.PostAsJsonAsync("Category/Create", category);
        if (response.IsSuccessStatusCode)
        {
            Console.WriteLine("Category added successfully.");
        }
        else
        {
            Console.WriteLine($"Failed to add category. Status Code: {response.StatusCode}");
        }
    }

    public async Task UpdateCategory(CategoryUpdateDto category)
    {
        var response = await _httpClient.PutAsJsonAsync("Category/Edit",category);
    }

}