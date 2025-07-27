using System.Net;
using System.Net.Http.Json;
using BlazorApp1.Entities.Dto;
using Xunit;

namespace BlazorApp1.IntegrationTests.Tests;

public class CategoryControllerTests : IClassFixture<IntegrationTestWebAppFactory<Program>>
{
    private readonly HttpClient _client;

    public CategoryControllerTests(IntegrationTestWebAppFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task CreateCategory_ReturnsOk_AndCategoryIsCreated()
    {
        // Arrange
        var newCategory = new CategoryCreateDto
        {
            Name = "IntegrationTestCategory"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/Category/Create", newCategory);
        var content = await response.Content.ReadAsStringAsync();

        Assert.True(response.IsSuccessStatusCode, $"Status: {response.StatusCode}, Body: {content}");


        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        // Optional: Confirm creation by retrieving all categories
        var categories = await _client.GetFromJsonAsync<List<CategoryViewDto>>("Category/GetAll");
        Assert.NotNull(categories);
        Assert.Contains(categories, c => c.Name == "IntegrationTestCategory");
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
    public async Task Create_Edit_Delete_Category_WorkFlow()
    {
        // 1. Create
        var newCategory = new CategoryCreateDto { Name = "TestCategory" };
        var createResponse = await _client.PostAsJsonAsync("Category/Create", newCategory);

        Assert.Equal(HttpStatusCode.OK, createResponse.StatusCode);

        // 2. Get all and verify new category is present
        var categories = await _client.GetFromJsonAsync<List<CategoryViewDto>>("Category/GetAll");
        Assert.Contains(categories!, c => c.Name == "TestCategory");

        var createdCategory = categories!.First(c => c.Name == "TestCategory");

        // 3. Edit
        var updateDto = new CategoryUpdateDto
        {
            Id = createdCategory.Id,
            Name = "UpdatedTestCategory"
        };
        var editResponse = await _client.PutAsJsonAsync("Category/Edit", updateDto);
        Assert.Equal(HttpStatusCode.OK, editResponse.StatusCode);

        // 4. Verify edit
        var categoriesAfterEdit = await _client.GetFromJsonAsync<List<CategoryViewDto>>("Category/GetAll");
        Assert.Contains(categoriesAfterEdit!, c => c.Name == "UpdatedTestCategory");

        // 5. Delete
        var deleteResponse = await _client.DeleteAsync($"Category/Delete?id={createdCategory.Id}");
        Assert.Equal(HttpStatusCode.OK, deleteResponse.StatusCode);

        // 6. Verify delete
        var categoriesAfterDelete = await _client.GetFromJsonAsync<List<CategoryViewDto>>("Category/GetAll");
        Assert.DoesNotContain(categoriesAfterDelete!, c => c.Id == createdCategory.Id);
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
