using BlazorApp1.Entities.Dto;
using BlazorApp1.WebAsembly.Services;
using Microsoft.AspNetCore.Components;

namespace BlazorApp1.WebAsembly.Pages.Templates;

public partial class AddBooking
{
    [Inject]
    private IBookingRepository repository { get; set; } = null!;

    public BookingCreateDto FormBooking { get; set; } = new BookingCreateDto
    {
        FromDate = DateTime.Today,
        ToDate = DateTime.Today
    };

    [Parameter]
    public EventCallback Added { get; set; }

    public async Task SubmitAsync()
    {
        await repository.AddBooking(FormBooking);
        await Added.InvokeAsync();
    }
}
