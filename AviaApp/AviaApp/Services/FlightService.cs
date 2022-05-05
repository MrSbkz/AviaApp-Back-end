using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AviaApp.Models.Requests;
using AviaApp.Models.ViewModels;
using AviaApp.Services.Contracts;
using Data;
using Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace AviaApp.Services;

public class FlightService : IFlightService
{
    private readonly IMapper _mapper;
    private readonly AviaAppDbContext _context;
    private readonly ICabinClassService _cabinClassService;

    public FlightService(IMapper mapper, AviaAppDbContext context, ICabinClassService cabinClassService)
    {
        _mapper = mapper;
        _context = context;
        _cabinClassService = cabinClassService;
    }

    public async Task<IList<FlightViewModel>> GetFlightsByDateRangeAsync(DateTime? dateFrom, DateTime? dateTo)
    {
        dateFrom ??= DateTime.MinValue;
        dateTo ??= DateTime.MaxValue;
        var flights = await _context.Flights
            .Include(x => x.AirportFrom.City.Country)
            .Include(x => x.AirportTo.City.Country)
            .Where(x =>
                x.DepartureDateTime.Date >= dateFrom.Value.Date && x.ArrivalDateTime.Date <= dateTo.Value.Date)
            .ToListAsync();

        return _mapper.Map<IList<FlightViewModel>>(flights);
    }

    public async Task<IList<FlightViewModel>> SearchFlightsAsync(SearchFlightRequest request)
    {
        var flightsByFromParameters = await GetFlightsFromBySearchParametersAsync(request.CountryIdFrom,
            request.CityIdFrom, request.AirportIdFrom, request.FlightDateTime);

        var flightsByToParameters = await GetFlightsToBySearchParametersAsync(request.CountryIdTo,
            request.CityIdTo, request.AirportIdTo, request.FlightDateTime);

        var flights = flightsByFromParameters.Where(x => flightsByToParameters.Any(y => y.Id.Equals(x.Id))).ToList();

        await SetPricesAsync(flights, request.CabinClassId);

        return _mapper.Map<IList<FlightViewModel>>(flights);
    }

    public async Task<FlightViewModel> GetFlightByIdAsync(Guid flightId)
    {
        var flight = await GetFlightIfExistsAsync(flightId);

        var flightDto = _mapper.Map<FlightViewModel>(flight);

        return flightDto;
    }

    public async Task<FlightViewModel> AddFlightAsync(AddFlightRequest request)
    {
        CheckDates(request.DepartureDateTime, request.ArrivalDateTime);

        if (request.AirportFromId.Equals(request.AirportToId))
            throw new Exception("You set the same airports");

        var flight = _mapper.Map<Flight>(request);
        await _context.Flights.AddAsync(flight);
        await _context.SaveChangesAsync();

        return await GetFlightByIdAsync(flight.Id);
    }

    public async Task AddFlightsAsync(IList<AddFlightRequest> request)
    {
        var flights = _mapper.Map<IList<Flight>>(request);
        await _context.Flights.AddRangeAsync(flights);
        await _context.SaveChangesAsync();
    }

    public async Task<FlightViewModel> UpdateFlightAsync(UpdateFlightRequest request)
    {
        var flight = await GetFlightIfExistsAsync(request.FlightId);

        if (flight.IsCanceled)
            throw new Exception("It is not possible to update canceled flight");

        flight.DepartureDateTime = request.DepartureDateTime ?? flight.DepartureDateTime;
        flight.ArrivalDateTime = request.ArrivalDateTime ?? flight.ArrivalDateTime;
        flight.Airplane = request.Airplane ?? flight.Airplane;
        flight.Price = request.Price ?? flight.Price;

        CheckDates(flight.DepartureDateTime, flight.ArrivalDateTime);

        _context.Flights.Update(flight);
        await _context.SaveChangesAsync();

        return await GetFlightByIdAsync(flight.Id);
    }

    public async Task DeleteFlightAsync(Guid flightId)
    {
        var flight = await GetFlightIfExistsAsync(flightId);
        _context.Flights.Remove(flight);
        await _context.SaveChangesAsync();
    }

    public async Task CancelFlightAsync(Guid flightId)
    {
        var flight = await GetFlightIfExistsAsync(flightId);
        flight.IsCanceled = true;
        _context.Flights.Update(flight);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteOutdatedFlightsAsync()
    {
        var outdatedFlights = await _context.Flights
            .Include(x => x.Bookings)
            .Where(x => x.DepartureDateTime < DateTime.Now.Date && !x.Bookings.Any())
            .ToListAsync();

        _context.RemoveRange(outdatedFlights);
        await _context.SaveChangesAsync();
    }

    private async Task<IList<Flight>> GetFlightsFromBySearchParametersAsync(
        Guid countryId,
        Guid? cityId,
        Guid? airportId,
        DateTime flightDateTime)
    {
        var flights = await _context.Flights.Include(x => x.AirportFrom.City.Country)
            .Where(x => !x.IsCanceled && x.ArrivalDateTime.Date.Equals(flightDateTime.Date) &&
                        x.AirportFrom.City.CountryId.Equals(countryId)).ToListAsync();

        if (airportId is not null)
            return flights.Where(x => x.AirportFromId.Equals(airportId)).ToList();

        return cityId is not null
            ? flights.Where(x => x.AirportFrom!.CityId.Equals(cityId)).ToList()
            : flights;
    }

    private async Task SetPricesAsync(IList<Flight> flights, int cabinClassId)
    {
        var cabinClass = await _cabinClassService.GetByIdAsync(cabinClassId);
        if (cabinClass is null)
            throw new Exception("The cabin class has not been found");

        foreach (var flight in flights)
        {
            flight.Price = GetPrice(flight.Price, cabinClass.PricePerCent);
        }
    }

    private decimal GetPrice(decimal price, int pricePerCent)
    {
        return price + (price / 100 * pricePerCent);
    }

    private async Task<IList<Flight>> GetFlightsToBySearchParametersAsync(
        Guid countryId,
        Guid? cityId,
        Guid? airportId,
        DateTime flightDateTime)
    {
        var flights = await _context.Flights.Include(x => x.AirportTo.City.Country)
            .Where(x => !x.IsCanceled && x.ArrivalDateTime.Date.Equals(flightDateTime.Date) &&
                        x.AirportTo.City.CountryId.Equals(countryId)).ToListAsync();

        if (airportId is not null)
            return flights.Where(x => x.AirportToId.Equals(airportId)).ToList();

        return cityId is not null
            ? flights.Where(x => x.AirportTo!.CityId.Equals(cityId)).ToList()
            : flights;
    }

    private void CheckDates(DateTime departureDateTime, DateTime arrivalDateTime)
    {
        if (departureDateTime.Date > arrivalDateTime.Date)
            throw new Exception(
                $"Incorrect dates: departure date({departureDateTime:yy-MM-dd}) more " +
                $"than arrival date({arrivalDateTime:yy-MM-dd})");
    }

    private async Task<Flight> GetFlightIfExistsAsync(Guid flightId)
    {
        var flight = await _context.Flights
            .Include(x => x.AirportFrom.City.Country)
            .Include(x => x.AirportTo.City.Country)
            .FirstOrDefaultAsync(x => x.Id == flightId);

        if (flight == null)
            throw new Exception("The flight has not been found");

        return flight;
    }
}