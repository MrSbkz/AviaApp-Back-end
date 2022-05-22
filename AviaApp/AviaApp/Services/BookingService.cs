using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AviaApp.Helpers;
using AviaApp.Models.Requests;
using AviaApp.Models.ViewModels;
using AviaApp.Services.Contracts;
using Data;
using Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace AviaApp.Services;

public class BookingService : IBookingService
{
    private readonly AviaAppDbContext _context;
    private readonly IMapper _mapper;
    private readonly ICabinClassService _cabinClassService;

    public BookingService(
        AviaAppDbContext context,
        IMapper mapper,
        ICabinClassService cabinClassService)
    {
        _context = context;
        _mapper = mapper;
        _cabinClassService = cabinClassService;
    }

    public async Task BookFlightAsync(BookFlightRequest request, string bookedBy)
    {
        if (!request.Passengers.Any())
            throw new Exception("Must have at least one passenger");

        var flight = await _context.Flights.FirstOrDefaultAsync(x => x.Id == request.FlightId);
        if (flight is null)
            throw new Exception("This flight does not exist");

        var cabinClass = await _cabinClassService.GetByIdAsync(request.CabinClassId);

        var booking = _mapper.Map<Booking>(request);
        booking.Id = Guid.NewGuid();
        booking.BookingDate = DateTime.Now;
        booking.BookedBy = bookedBy;
        booking.Price = PriceHelper.GetPrice(flight.Price, cabinClass.PricePerCent);

        await _context.Bookings.AddAsync(booking);
        await _context.SaveChangesAsync();
    }

    public async Task CancelBookingAsync(Guid bookingId)
    {
        var booking = await _context.Bookings.Include(x => x.Passengers).FirstOrDefaultAsync(x => x.Id == bookingId);
        if (booking is null)
            throw new Exception("The booking is not found");

        booking.IsCanceled = true;
        if (booking.Passengers != null)
        {
            var cancelDate = DateTime.Now;
            foreach (var passenger in booking.Passengers)
            {
                passenger.IsCanceled = true;
                passenger.CancelDate = cancelDate;
            }
        }

        _context.Bookings.Update(booking);
        await _context.SaveChangesAsync();
    }

    public async Task CancelBookingForPassengerAsync(Guid passengerId)
    {
        var passenger = await _context.Passengers.FirstOrDefaultAsync(x => x.Id == passengerId);
        if (passenger is null)
            throw new Exception("The passenger is not found");

        passenger.IsCanceled = true;
        passenger.CancelDate = DateTime.Now;
        _context.Passengers.Update(passenger);

        var booking = await _context.Bookings.Include(x => x.Passengers).FirstAsync(x => x.Id == passenger.BookingId);
        if (booking.Passengers!.All(x => x.IsCanceled))
        {
            booking.IsCanceled = true;
            _context.Bookings.Update(booking);
        }

        await _context.SaveChangesAsync();
    }

    public async Task<IList<BookingViewModel>> GetBookingsByEmailAsync(string email)
    {
        var bookings = await _context.Bookings
            .Include(x => x.Passengers)
            .Include(x => x.CabinClass)
            .Include(x => x.Flight.AirportFrom.City.Country)
            .Include(x => x.Flight.AirportTo.City.Country)
            .Where(x => x.BookedBy.Equals(email) && !x.Flight.IsCanceled)
            .ToListAsync();

        return _mapper.Map<IList<BookingViewModel>>(bookings);
    }
}