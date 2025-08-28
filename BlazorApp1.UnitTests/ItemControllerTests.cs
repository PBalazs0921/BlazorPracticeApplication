using System.Security.Claims;
using BlazorApp1.Data.Helper;
using BlazorApp1.Entities.Dto;
using BlazorApp1.Endpoint.Controllers;
using BlazorApp1.Logic.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace  UnitTests;

public class ItemControllerTests
{
    private readonly Mock<IItemLogic> _mockLogic;
    private readonly ItemController _controller;

    public ItemControllerTests()
    {
        _mockLogic = new Mock<IItemLogic>();

        var store = new Mock<IUserStore<AppUser>>();
        var mockUserManager = new Mock<UserManager<AppUser>>(store.Object, null!, null!, null!, null!, null!, null!, null!, null!);
        var adminUser = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.NameIdentifier, "admin-id"),
            new Claim(ClaimTypes.Name, "admin"),
            new Claim(ClaimTypes.Role, "Admin")
        }, "mock"));
        // Create controller and assign user
        _controller = new ItemController(mockUserManager.Object, _mockLogic.Object)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = adminUser }
            }
        };
        mockUserManager.Setup(x => x.GetUserAsync(adminUser))
            .ReturnsAsync(new AppUser
            {
                Id = "admin-id",
                UserName = "admin",
                FamilyName = null,
                GivenName = null,
                RefreshToken = null!,
                RefreshTokenExpiryTime = DateTime.Now
            });
    }

    [Fact]
    public async Task Get_ShouldReturnItems()
    {
        // Arrange
        var items = new List<ItemViewDto>
        {
            new() { Id = 1, Name = "Item1" },
            new() { Id = 2, Name = "Item2" }
        };
        _mockLogic.Setup(x => x.GetAllItemsAsync()).ReturnsAsync(items);

        // Act
        var result = await _controller.Get();

        // Assert
        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task Create_ShouldReturnOk_WhenSuccess()
    {
        // Arrange
        var createDto = new ItemCreateDto { Name = "NewItem" };
        _mockLogic.Setup(x => x.CreateItemAsync(createDto)).ReturnsAsync(1);

        // Act
        var result = await _controller.Create(createDto);

        // Assert
        Assert.IsType<OkResult>(result);
    }

    [Fact]
    public async Task Create_ShouldReturnBadRequest_WhenFail()
    {
        // Arrange
        var createDto = new ItemCreateDto { Name = "NewItem" };
        _mockLogic.Setup(x => x.CreateItemAsync(createDto)).ReturnsAsync((int?)null);

        // Act
        var result = await _controller.Create(createDto);

        // Assert
        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Failed to create item.", badRequest.Value);
    }

    [Fact]
    public async Task Edit_ShouldReturnOk_WhenUpdateSucceeds()
    {
        // Arrange
        var updateDto = new ItemUpdateDto { Id = 1, Name = "Updated" };
        _mockLogic.Setup(x => x.UpdateItemAsync(updateDto)).ReturnsAsync(true);

        // Act
        var result = await _controller.Edit(updateDto);

        // Assert
        Assert.IsType<OkResult>(result);
    }

    [Fact]
    public async Task Edit_ShouldReturnNotFound_WhenUpdateFails()
    {
        // Arrange
        var updateDto = new ItemUpdateDto { Id = 1, Name = "Updated" };
        _mockLogic.Setup(x => x.UpdateItemAsync(updateDto)).ReturnsAsync(false);

        // Act
        var result = await _controller.Edit(updateDto);

        // Assert
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
