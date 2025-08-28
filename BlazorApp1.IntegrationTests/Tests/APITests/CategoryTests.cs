using System.Net;
using System.Net.Http.Json;
using BlazorApp1.Entities.Dto;
using Xunit;

namespace BlazorApp1.IntegrationTests.Tests.APITests;

[Collection("ApiTestCollection")]
public class CategoryControllerTests 
{
    private readonly HttpClient _client;

    public CategoryControllerTests(IntegrationTestWebAppFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task CreateCategory_Returns_Unauthorized()
    {
    // Arrange
        var newCategory = new CategoryCreateDto
        {
            Name = "IntegrationTestCategory"
        };

    // Act
        var response = await _client.PostAsJsonAsync("/Category/Create", newCategory);
        var content = await response.Content.ReadAsStringAsync();

    // Assert that the response is Unauthorized
        Assert.False(response.IsSuccessStatusCode, $"Expected Unauthorized but got Status: {response.StatusCode}, Body: {content}");
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        

    }
    [Fact]
    public async Task GetAll_ReturnsCategories()
    {
        // Act
        var response = await _client.GetAsync("Category/GetAll");
        
        // Assert status code
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        // Deserialize response
        var categories = await response.Content.ReadFromJsonAsync<List<CategoryViewDto>>();
        Assert.NotNull(categories);
        Assert.NotEmpty(categories); // assuming seeded data or at least one entry
    }
    

    [Fact]
    public async Task Edit_NonExistingCategory_ReturnsNotFound()
    {
        var updateDto = new CategoryUpdateDto
        {
            Id = 999999, // some non-existing id
            Name = "NonExistent"
        };
        var response = await _client.PutAsJsonAsync("Category/Edit", updateDto);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
