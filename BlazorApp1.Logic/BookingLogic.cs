using AutoMapper;
using BlazorApp1.Data;
using BlazorApp1.Entities.Dto;
using BlazorApp1.Entities.Entity;
using BlazorApp1.Logic.Dto;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace BlazorApp1.Logic;

public class BookingLogic(
    Repository<Booking> repository,
    IUnitOfWork uow,
    IMemoryCache cache,
    DtoProvider dtoProvider)
{
    private const string AllBookingsCacheKey = "bookings_all";

    // Shorter than other lists because BookingViewDto embeds a UserName snapshot.
    // A user rename isn't observed here, so this list can serve a stale name
    // until it expires — bounded to a few seconds.
    private static readonly TimeSpan CacheDuration = TimeSpan.FromSeconds(10);

    private readonly Mapper _mapper = dtoProvider.Mapper;

    public async Task<IEnumerable<BookingViewDto>> GetAllBookingsAsync()
    {
        return await cache.GetOrCreateAsync(AllBookingsCacheKey, async entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = CacheDuration;
            var bookings = await repository.GetAllAsync();
            return bookings.Select(x => _mapper.Map<BookingViewDto>(x)).ToList();
        }) ?? [];
    }

    private bool IsBookingValid(Booking booking, int? editId = null)
    {
        Console.WriteLine("checking booking validity");
        //From date has to be earlier than to date
        if (booking.FromDate.Date > booking.ToDate.Date)
        {
            return false;
        }
        
        var bookings = repository.GetAll()
            .Where(x => x.ItemId == booking.ItemId && x.Id != editId)
            .ToList();
        if (bookings.Count == 0)
        {
            //If there are no booking for the item, the booking is valid
            return true;
        }
        else
        {
            Console.WriteLine("found: " + bookings.Count + " bookings");
            //if there are bookings, we have checked.
            //If the latest toDate is equals, or 1 day before the current. its valid

            var latestBooking = bookings.OrderByDescending(x => x.ToDate).First();
            var earliestBooking = bookings.OrderByDescending(x => x.ToDate).Last();
            
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
            uow.Create(item);
            await uow.SaveChangesAsync();
            cache.Remove(AllBookingsCacheKey);
            return true;
        }

        throw new Exception("Booking is not valid");
        
    }
    
    public async Task<bool> UpdateBookingAsync(BookingUpdateDto booking)
    {
        Console.WriteLine("UpdateBookingAsync called");
        var item = await uow.Any<Booking>()
            .FirstOrDefaultAsync(x => x.Id == booking.Id);
        if (item == null) throw new Exception("Booking not found");

        if (IsBookingValid(_mapper.Map<Booking>(booking), booking.Id))
        {
            _mapper.Map(booking, item);
            await uow.SaveChangesAsync();
            cache.Remove(AllBookingsCacheKey);
            return true;
        }

        throw new Exception("Booking is not valid");
        
    }

    public async Task<bool> DeleteItemAsync(int id)
    {
        var item = await uow.Any<Booking>()
            .FirstOrDefaultAsync(x => x.Id == id);
        if (item == null) return false;

        uow.Remove(item);
        await uow.SaveChangesAsync();
        cache.Remove(AllBookingsCacheKey);
        return true;
    }

}