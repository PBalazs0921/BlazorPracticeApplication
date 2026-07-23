using System.Net.Http.Json;
using BlazorApp1.Entities.Dto;

namespace BlazorApp1.WebAsembly.Services;

public class BookingRepository : IBookingRepository
{
    private readonly HttpClient _httpClient;

    public BookingRepository(IHttpClientFactory factory)
    {
        _httpClient = factory.CreateClient("ServerApi");
    }

    public async Task<List<BookingViewDto>> GetBookings()
    {
        var response = await _httpClient.GetAsync("Booking/GetAll");
        if (response.IsSuccessStatusCode)
        {
            var bookings = await response.Content.ReadFromJsonAsync<List<BookingViewDto>>();
            return bookings ?? new List<BookingViewDto>();
        }

        throw new Exception("Failed to load bookings");
    }

    public async Task DeleteBooking(int id)
    {
        var response = await _httpClient.DeleteAsync($"Booking/Delete?id={id}");
        if (!response.IsSuccessStatusCode)
        {
            Console.WriteLine($"Failed to delete booking with Id: {id}. Status Code: {response.StatusCode}");
        }
    }

    public async Task AddBooking(BookingCreateDto booking)
    {
        var response = await _httpClient.PostAsJsonAsync("Booking/Create", booking);
        if (!response.IsSuccessStatusCode)
        {
            Console.WriteLine($"Failed to add booking. Status Code: {response.StatusCode}");
        }
    }

    public async Task UpdateBooking(BookingUpdateDto booking)
    {
        var response = await _httpClient.PutAsJsonAsync("Booking/Update", booking);
        if (!response.IsSuccessStatusCode)
        {
            Console.WriteLine($"Failed to update booking with Id: {booking.Id}. Status Code: {response.StatusCode}");
        }
    }
}
