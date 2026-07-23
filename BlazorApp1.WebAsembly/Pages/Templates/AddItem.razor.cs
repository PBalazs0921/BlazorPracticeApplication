using BlazorApp1.Entities.Dto;
using BlazorApp1.WebAsembly.Services;
using Microsoft.AspNetCore.Components;

namespace BlazorApp1.WebAsembly.Pages.Templates;

public partial class AddItem
{
    [Inject]
    private IItemRepository repository { get; set; } = null!;

    public ItemCreateDto FormItem { get; set; } = new ItemCreateDto();

    [Parameter]
    public EventCallback Added { get; set; }

    public async Task SubmitAsync()
    {
        await repository.AddItem(FormItem);
        await Added.InvokeAsync();
    }
}
