using BlazorApp1.Data.Helper;
using BlazorApp1.Endpoint.Controllers;
using BlazorApp1.Entities.Dto;
using BlazorApp1.Logic.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace UnitTests;

public class CategoryControllerTests
{
    private readonly CategoryController _controller;
    private readonly Mock<UserManager<AppUser>> _mockUserManager;
    private readonly Mock<ICategoryLogic> _mockCategoryLogic;

    public CategoryControllerTests()
    {
        var store = new Mock<IUserStore<AppUser>>();
        _mockUserManager = new Mock<UserManager<AppUser>>(
            store.Object, null, null, null, null, null, null, null, null
        );

        _mockCategoryLogic = new Mock<ICategoryLogic>();
        _controller = new CategoryController(_mockUserManager.Object, _mockCategoryLogic.Object);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllCategories()
    {
        // Arrange
        var categories = new List<CategoryViewDto>
        {
            new CategoryViewDto { Id = 1, Name = "Test1" },
            new CategoryViewDto { Id = 2, Name = "Test2" }
        };

        _mockCategoryLogic.Setup(x => x.GetAllItemsAsync()).ReturnsAsync(categories);

        // Act
        var result = await _controller.GetAllAsync();

        // Assert
        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task Edit_ShouldReturnOk_WhenUpdateSucceeds()
    {
        // Arrange
        var updateDto = new CategoryUpdateDto { Id = 1, Name = "Updated" };
        _mockCategoryLogic.Setup(x => x.UpdateItemAsync(updateDto)).ReturnsAsync(true);

        // Act
        var result = await _controller.Edit(updateDto);

        // Assert
        Assert.IsType<OkResult>(result);
    }

    [Fact]
    public async Task Edit_ShouldReturnNotFound_WhenUpdateFails()
    {
        // Arrange
        var updateDto = new CategoryUpdateDto { Id = 99, Name = "Invalid" };
        _mockCategoryLogic.Setup(x => x.UpdateItemAsync(updateDto)).ReturnsAsync(false);

        // Act
        var result = await _controller.Edit(updateDto);

        // Assert
        var notFound = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal("Category not found or update failed.", notFound.Value);
    }

    
    [Fact]
    public async Task Create_ShouldReturnBadRequest_WhenCreationFails()
    {
        // Arrange
        var createDto = new CategoryCreateDto { Name = "New" };

        // Mock CreateItemAsync to return null (meaning failure)
        _mockCategoryLogic.Setup(x => x.CreateItemAsync(createDto)).ReturnsAsync((CategoryViewDto?)null);

        // Act
        var result = await _controller.Create(createDto);

        // Assert
        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Failed to create category.", badRequest.Value);
    }


    [Fact]
    public async Task Create_ShouldReturnOk_WhenCreationSucceeds()
    {
        // Arrange
        var createDto = new CategoryCreateDto { Name = "New" };
        var successResult = new CategoryViewDto { Id = 2, Name = "TEST" }; // non-null Id means success

        _mockCategoryLogic.Setup(x => x.CreateItemAsync(createDto)).ReturnsAsync(successResult);

        // Act
        var result = await _controller.Create(createDto);

        // Assert
        Assert.IsType<OkObjectResult>(result);
    }


    [Fact]
    public async Task Delete_ShouldReturnTrue_WhenCategoryIsDeleted()
    {
        // Arrange
        int id = 1;
        _mockCategoryLogic.Setup(x => x.DeleteCategoryAsync(id)).ReturnsAsync(true);

        // Act
        var result = await _controller.Delete(id);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task Delete_ShouldReturnFalse_WhenCategoryNotFound()
    {
        // Arrange
        int id = 999;
        _mockCategoryLogic.Setup(x => x.DeleteCategoryAsync(id)).ReturnsAsync(false);

        // Act
        var result = await _controller.Delete(id);

        // Assert
        Assert.False(result);
    }
}