using BlazorApp1.Entities.Dto;
using BlazorApp1.WebAsembly.Services;
using Microsoft.AspNetCore.Components;

namespace BlazorApp1.WebAsembly.Pages;

public partial class Items
{
    [Inject]
    private IItemRepository Repository { get; set; } = null!;

    private List<ItemViewDto>? _items;
    private bool _isEditMode = false;
    private bool _isAddMode = false;
    private ItemUpdateDto? _editItem;

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        await LoadItems();
    }

    private async Task LoadItems()
    {
        _items = await Repository.GetItems();
    }

    public async Task ItemAdded()
    {
        _isAddMode = false;
        await LoadItems();
    }

    private async Task DeleteItem(int id)
    {
        await Repository.DeleteItem(id);
        await LoadItems();
    }

    private void StartEdit(int id)
    {
        var current = _items?.FirstOrDefault(x => x.Id == id);
        if (current != null)
        {
            _editItem = new ItemUpdateDto
            {
                Id = current.Id,
                Name = current.Name,
                CategoryId = current.CategoryId,
                Price = current.Price
            };
            _isEditMode = true;
        }
    }

    private async Task HandleEditItem()
    {
        if (_editItem != null)
        {
            await Repository.UpdateItem(_editItem);
            _isEditMode = false;
            _editItem = null;
            await LoadItems();
        }
    }

    private void CancelEdit()
    {
        _isEditMode = false;
        _editItem = null;
    }
}
