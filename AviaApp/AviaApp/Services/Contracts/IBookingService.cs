using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AviaApp.Models.Requests;
using AviaApp.Models.ViewModels;

namespace AviaApp.Services.Contracts;

public interface IBookingService
{
    Task BookFlightAsync(BookFlightRequest request, string bookedBy);

    Task CancelBookingAsync(Guid bookingId);

    Task CancelBookingForPassengerAsync(Guid passengerId);

    Task<IList<BookingViewModel>> GetBookingsByEmailAsync(string email);
}