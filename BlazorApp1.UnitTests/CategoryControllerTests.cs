using System.Security.Claims;
using BlazorApp1.Endpoint.Controllers;
using BlazorApp1.Entities.Dto;
using BlazorApp1.Logic.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace UnitTests;

public class CategoryControllerTests
{
    private readonly CategoryController _controller;
    private readonly Mock<ICategoryLogic> _mockCategoryLogic;

    public CategoryControllerTests()
    {
        _mockCategoryLogic = new Mock<ICategoryLogic>();

        _controller = new CategoryController(_mockCategoryLogic.Object)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(new[]
                    {
                        new Claim("sub", "auth0|admin-id")
                    }, "mock"))
                }
            }
        };
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllCategories()
    {
        var categories = new List<CategoryViewDto>
        {
            new CategoryViewDto { Id = 1, Name = "Test1" },
            new CategoryViewDto { Id = 2, Name = "Test2" }
        };
        _mockCategoryLogic.Setup(x => x.GetAllItemsAsync()).ReturnsAsync(categories);

        var result = await _controller.GetAllAsync();

        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task Edit_ShouldReturnOk_WhenUpdateSucceeds()
    {
        var updateDto = new CategoryUpdateDto { Id = 1, Name = "Updated" };
        _mockCategoryLogic.Setup(x => x.UpdateItemAsync(updateDto)).ReturnsAsync(true);

        var result = await _controller.Edit(updateDto);

        Assert.IsType<OkResult>(result);
    }

    [Fact]
    public async Task Edit_ShouldReturnNotFound_WhenUpdateFails()
    {
        var updateDto = new CategoryUpdateDto { Id = 99, Name = "Invalid" };
        _mockCategoryLogic.Setup(x => x.UpdateItemAsync(updateDto)).ReturnsAsync(false);

        var result = await _controller.Edit(updateDto);

        var notFound = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal("Category not found or update failed.", notFound.Value);
    }

    [Fact]
    public async Task Create_ShouldReturnBadRequest_WhenCreationFails()
    {
        var createDto = new CategoryCreateDto { Name = "New" };
        _mockCategoryLogic.Setup(x => x.CreateItemAsync(createDto)).ReturnsAsync((CategoryViewDto?)null);

        var result = await _controller.Create(createDto);

        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Failed to create category.", badRequest.Value);
    }

    [Fact]
    public async Task Create_ShouldReturnOk_WhenCreationSucceeds()
    {
        var createDto = new CategoryCreateDto { Name = "New" };
        _mockCategoryLogic.Setup(x => x.CreateItemAsync(createDto)).ReturnsAsync(new CategoryViewDto { Id = 2, Name = "TEST" });

        var result = await _controller.Create(createDto);

        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task Delete_ShouldReturnOk_WhenCategoryIsDeleted()
    {
        _mockCategoryLogic.Setup(x => x.DeleteCategoryAsync(1)).ReturnsAsync(true);

        var result = await _controller.Delete(1);

        Assert.IsType<OkResult>(result);
    }

    [Fact]
    public async Task Delete_ShouldReturnNotFound_WhenCategoryNotFound()
    {
        _mockCategoryLogic.Setup(x => x.DeleteCategoryAsync(999)).ReturnsAsync(false);

        var result = await _controller.Delete(999);

        Assert.IsType<NotFoundObjectResult>(result);
    }
}
