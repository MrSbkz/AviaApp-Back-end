using AutoMapper;
using AviaApp.Models.Requests;
using AviaApp.Models.ViewModels;
using Data.Entities;

namespace AviaApp.Mapper.Profiles;

public class BookingProfile : Profile
{
    public BookingProfile()
    {
        CreateMap<BookFlightRequest, Booking>();
        CreateMap<Booking, BookFlightRequest>();

        CreateMap<PassengerRequest, Passenger>();
        CreateMap<Passenger, PassengerRequest>();

        CreateMap<Booking, BookingViewModel>();
        CreateMap<BookingViewModel, Booking>();

        CreateMap<Passenger, PassengerViewModel>();
        CreateMap<PassengerViewModel, Passenger>();
    }
}