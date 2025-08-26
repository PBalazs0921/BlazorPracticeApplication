using AutoMapper;
using BlazorApp1.Data;
using BlazorApp1.Entities.Dto;
using BlazorApp1.Entities.Entity;
using BlazorApp1.Logic.Dto;

namespace BlazorApp1.Logic;

public class BookingLogic(Repository<Booking> repository, DtoProvider dtoProvider)
{
    private readonly Mapper _mapper = dtoProvider.Mapper;

    public async Task<IEnumerable<BookingViewDto>> GetAllBookingsAsync()
    {
        Console.WriteLine("GetAllItems called");
        var bookings = await repository.GetAllAsync();
        return bookings.Select(x => _mapper.Map<BookingViewDto>(x));
    }

    private bool IsBookingValid(Booking booking, int? editId = null)
    {
        Console.WriteLine("checking booking validity");
        //From date has to be earlier than to date
        if (booking.FromDate.Date > booking.ToDate.Date)
        {
            return false;
        }
        
        var Bookings = repository.GetAll().Where(x => x.ItemId == booking.ItemId && x.Id != editId);
        if (!Bookings.Any())
        {
            //If there are no booking for the item, the booking is valid
            return true;
        }
        else
        {
            Console.WriteLine("found: " + Bookings.Count() + " bookings");
            //if there are bookings, we have checked.
            //If the latest toDate is equals, or 1 day before the current. its valid
            
            var latestBooking = Bookings.OrderByDescending(x => x.ToDate).First();
            var earliestBooking = Bookings.OrderByDescending(x => x.ToDate).Last();
            
            //We assume that the item can be lent out on the same day

            
            // Check if the latest booking's ToDate is equal to or 1 day before the currentBooking's from date
            Console.WriteLine("LatestTO currentFROM" + latestBooking.ToDate.Date + " " + booking.FromDate.Date);
            Console.WriteLine("EarliestFROM currentTO" + earliestBooking.FromDate.Date + " " + booking.ToDate.Date);
            if (latestBooking.ToDate.Date == booking.FromDate.Date || latestBooking.ToDate.Date.AddDays(1) == booking.FromDate.Date)
            {
                return true;
            }
            
            // if the earliest booking's from date is equal to or 1 day before the currentBooking's to date
            if (earliestBooking.FromDate.Date == booking.ToDate.Date || earliestBooking.FromDate.Date == booking.ToDate.Date.AddDays(1))
            {
                return true;
            }
            
            
            return false;
        }
    }
    
    public async Task<bool> CreateBookingAsync(BookingCreateDto dto)
    {
        var item = _mapper.Map<Booking>(dto);
        if (IsBookingValid(item))
        {
            await repository.CreateAsync(item);
            return true;
        }
        else
        {
            throw new Exception("Booking is not valid");
        }
        
    }
    
    public async Task<bool> UpdateBookingAsync(BookingUpdateDto booking)
    {
        Console.WriteLine("UpdateBookingAsync called");
        var item = await repository.FindByIdAsync(booking.Id);
        if (item == null)             throw new Exception("Booking not found");

        if (IsBookingValid(_mapper.Map<Booking>(booking), booking.Id))
        {
            _mapper.Map(booking, item);
            await repository.UpdateAsync(item);
            return true;
        }

        throw new Exception("Booking is not valid");
        
    }

    public async Task<bool> DeleteItemAsync(int id)
    {
        var item = await repository.FindByIdAsync(id);
        if (item == null) return false;
        await repository.DeleteAsync(item);
        return true;
    }
    
}