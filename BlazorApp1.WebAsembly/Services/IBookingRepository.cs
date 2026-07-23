using BlazorApp1.Entities.Dto;

namespace BlazorApp1.WebAsembly.Services;

public interface IBookingRepository
{
    Task<List<BookingViewDto>> GetBookings();
    Task DeleteBooking(int id);
    Task AddBooking(BookingCreateDto booking);
    Task UpdateBooking(BookingUpdateDto booking);
}
