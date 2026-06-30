using System.Security.Claims;
using BlazorApp1.Entities.Dto;
using BlazorApp1.Endpoint.Controllers;
using BlazorApp1.Logic.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace UnitTests;

public class ItemControllerTests
{
    private readonly Mock<IItemLogic> _mockLogic;
    private readonly ItemController _controller;

    public ItemControllerTests()
    {
        _mockLogic = new Mock<IItemLogic>();

        _controller = new ItemController(_mockLogic.Object)
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
    public async Task Get_ShouldReturnItems()
    {
        var items = new List<ItemViewDto>
        {
            new() { Id = 1, Name = "Item1" },
            new() { Id = 2, Name = "Item2" }
        };
        _mockLogic.Setup(x => x.GetAllItemsAsync()).ReturnsAsync(items);

        var result = await _controller.Get();

        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task Create_ShouldReturnOk_WhenSuccess()
    {
        var createDto = new ItemCreateDto { Name = "NewItem" };
        _mockLogic.Setup(x => x.CreateItemAsync(createDto)).ReturnsAsync(1);

        var result = await _controller.Create(createDto);

        Assert.IsType<OkResult>(result);
    }

    [Fact]
    public async Task Create_ShouldReturnBadRequest_WhenFail()
    {
        var createDto = new ItemCreateDto { Name = "NewItem" };
        _mockLogic.Setup(x => x.CreateItemAsync(createDto)).ReturnsAsync((int?)null);

        var result = await _controller.Create(createDto);

        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Failed to create item.", badRequest.Value);
    }

    [Fact]
    public async Task Edit_ShouldReturnOk_WhenUpdateSucceeds()
    {
        var updateDto = new ItemUpdateDto { Id = 1, Name = "Updated" };
        _mockLogic.Setup(x => x.UpdateItemAsync(updateDto)).ReturnsAsync(true);

        var result = await _controller.Edit(updateDto);

        Assert.IsType<OkResult>(result);
    }

    [Fact]
    public async Task Edit_ShouldReturnNotFound_WhenUpdateFails()
    {
        var updateDto = new ItemUpdateDto { Id = 1, Name = "Updated" };
        _mockLogic.Setup(x => x.UpdateItemAsync(updateDto)).ReturnsAsync(false);

        var result = await _controller.Edit(updateDto);

        var notFound = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal("Item not found or update failed.", notFound.Value);
    }

    [Fact]
    public async Task Delete_ShouldReturnOk_WhenDeleteSucceeds()
    {
        _mockLogic.Setup(x => x.DeleteItemAsync(1)).ReturnsAsync(true);

        var result = await _controller.Delete(1);

        Assert.IsType<OkResult>(result);
    }

    [Fact]
    public async Task Delete_ShouldReturnNotFound_WhenDeleteFails()
    {
        _mockLogic.Setup(x => x.DeleteItemAsync(1)).ReturnsAsync(false);

        var result = await _controller.Delete(1);

        var notFound = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal("Item not found or delete failed.", notFound.Value);
    }
}
