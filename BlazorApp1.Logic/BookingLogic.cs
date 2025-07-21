using AutoMapper;
using BlazorApp1.Data;
using BlazorApp1.Entities;
using BlazorApp1.Entities.Dto;

namespace BlazorApp1.Logic;

public class BookingLogic
{
    private readonly Repository<Booking> _repository;
    public Mapper mapper;

    public BookingLogic(Repository<Booking> repository,DtoProvider dtoProvider)
    {
        _repository = repository;
        mapper = dtoProvider.mapper;
    }

    public IEnumerable<BookingViewDto> GetAllBookings()
    {
        Console.WriteLine("GetAllItems called");
        return _repository.GetAll().Select(x=> mapper.Map<BookingViewDto>(x));
    }

    private bool IsBookingValid(Booking booking, int? editId = null)
    {
        Console.WriteLine("checking booking validity");
        //From date has to be earlier than to date
        if (booking.FromDate.Date > booking.ToDate.Date)
        {
            return false;
        }
        
        var Bookings = _repository.GetAll().Where(x => x.ItemId == booking.ItemId && x.Id != editId);
        if (Bookings.Count() < 1)
        {
            //If there are no booking for the item, the booking is valid
            return true;
        }
        else
        {
            Console.WriteLine("found: " + Bookings.Count() + " bookings");
            //if there are bookings, we have check.
            //If the latest toDate is equals, or 1 day before the current. its valid
            
            var latestBooking = Bookings.OrderByDescending(x => x.ToDate).FirstOrDefault();
            var earliestBooking = Bookings.OrderByDescending(x => x.ToDate).LastOrDefault();
            
            //We assume that the item can be lent out on the same day
            
            // Check if the latest booking's ToDate is equal to or 1 day before the currentBooking's from date
            Console.WriteLine("LatestTO currentFROM" + latestBooking.ToDate.Date + " " + booking.FromDate.Date);
            Console.WriteLine("EarliestFROM currentTO" + earliestBooking.FromDate.Date + " " + booking.ToDate.Date);
            if (latestBooking.ToDate.Date == booking.FromDate.Date || latestBooking.ToDate.Date.AddDays(1) == booking.FromDate.Date)
            {
                return true;
            }
            
            // if the earlyest booking's from date is equal to or 1 day before the currentBooking's to date
            if (earliestBooking.FromDate.Date == booking.ToDate.Date || earliestBooking.FromDate.Date == booking.ToDate.Date.AddDays(1))
            {
                return true;
            }
            
            
            return false;
        }
    }

    public bool CreateBooking(BookingCreateDto dto)
    {
        Console.WriteLine("CreateBooking called");
        
        var NewBooking = mapper.Map<Booking>(dto);

        if (IsBookingValid(NewBooking))
        {
            _repository.Create(NewBooking);
            return true;
        }
        else
        {
            throw new Exception("Booking is not valid");
        }
    }

    public bool UpdateBooking(BookingUpdateDto booking)
    {
        Console.WriteLine("UpdateBooking called");
        var item = _repository.FindById(booking.Id);
        Console.WriteLine("ID of the booking that I update" + booking.Id);
        if (item == null) return false;
        // Update the properties of the item
        
        var BookingToUpdate = _repository.FindById(booking.Id);
        if (BookingToUpdate != null && BookingToUpdate.Id == booking.Id)
        {
            mapper.Map(booking, BookingToUpdate);
            _repository.Update(BookingToUpdate);
        }
        else
        {
            throw new Exception("Booking not found");
        }
        return true;
    }
    
    public bool DeleteItem(int id)
    {
        Console.WriteLine("DeleteItem pressed: " + id);
        var item = _repository.FindById(id);
        if (item == null) return false;
        _repository.Delete(item);
        return true;
    }
}