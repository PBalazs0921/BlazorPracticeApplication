using BlazorApp1.Entities.Dto;
using BlazorApp1.WebAsembly.Services;
using Microsoft.AspNetCore.Components;

namespace BlazorApp1.WebAsembly.Pages;

public partial class Categories
{
    [Inject]
    private ICategoryRepository Repository { get; set; } = null;
    private List<CategoryViewDto>? _categories;
    private bool _isEditMode = false;
    private bool _isAddMode =false;
    private IEnumerable<Categories> categories = new List<Categories>();
    private CategoryCreateDto _newCategory = new CategoryCreateDto();
    private CategoryUpdateDto? _editCategory;

    public async Task CategoryAdded()
    {
        _isAddMode = false;
        await LoadCategories();
    }

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        await LoadCategories();
    }
    
    private async Task LoadCategories()
    {
        _categories = await Repository.GetCategories();
    }
    
    private async Task DeleteItem(int id)
    {
        Console.WriteLine($"Delete button clicked for category with Id: {id}");
        await Repository.DeleteItem(id);
    }

    private async Task AddItem(CategoryCreateDto category)
    {
        await Repository.AddCategory(category);
    }

    private async Task<CategoryUpdateDto> EditItem(CategoryUpdateDto category)
    {
        var curItem = _categories?.FirstOrDefault(x => x.Id == category.Id);
        if (curItem != null)
        {
            _editCategory = new CategoryUpdateDto
            {
                Id = curItem.Id,
                Name = curItem.Name
            };
            _isEditMode = true;
        }
        
        return _editCategory;
    }
    
    private async Task HandleEditItem()
    {
    }
    private void CancelEdit()
    {
        // Cancel editing and return to the list
        _isEditMode = false;
    }
}