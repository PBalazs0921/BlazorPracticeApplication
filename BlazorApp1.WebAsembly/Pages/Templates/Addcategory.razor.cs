using BlazorApp1.Entities.Dto;
using BlazorApp1.WebAsembly.Services;
using Microsoft.AspNetCore.Components;

namespace BlazorApp1.WebAsembly.Pages.Templates;

public partial class AddCategory
{
    [Inject]
    private ICategoryRepository repository { get; set; } = null!;

    public CategoryCreateDto FormCategory { get; set; } = new CategoryCreateDto();

    [Parameter]
    public EventCallback Added { get; set; }
    public async Task SubmitAsync()
    {
        Console.WriteLine("ADD CALLED");
        await repository.AddCategory(FormCategory);
        await Added.InvokeAsync();
    }
}