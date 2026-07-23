using BlazorApp1.Entities.Dto;
using BlazorApp1.WebAsembly.Services;
using Microsoft.AspNetCore.Components;

namespace BlazorApp1.WebAsembly.Pages;

public partial class Bookings
{
    [Inject]
    private IBookingRepository Repository { get; set; } = null!;

    private List<BookingViewDto>? _bookings;
    private bool _isEditMode = false;
    private bool _isAddMode = false;
    private BookingUpdateDto? _editBooking;

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        await LoadBookings();
    }

    private async Task LoadBookings()
    {
        _bookings = await Repository.GetBookings();
    }

    public async Task BookingAdded()
    {
        _isAddMode = false;
        await LoadBookings();
    }

    private async Task DeleteBooking(int id)
    {
        await Repository.DeleteBooking(id);
        await LoadBookings();
    }

    private void StartEdit(int id)
    {
        var current = _bookings?.FirstOrDefault(x => x.Id == id);
        if (current != null)
        {
            _editBooking = new BookingUpdateDto
            {
                Id = current.Id,
                FromDate = current.FromDate,
                ToDate = current.ToDate,
                UserId = current.UserId,
                ItemId = current.ItemId,
                Comment = current.Comment
            };
            _isEditMode = true;
        }
    }

    private async Task HandleEditBooking()
    {
        if (_editBooking != null)
        {
            await Repository.UpdateBooking(_editBooking);
            _isEditMode = false;
            _editBooking = null;
            await LoadBookings();
        }
    }

    private void CancelEdit()
    {
        _isEditMode = false;
        _editBooking = null;
    }
}
