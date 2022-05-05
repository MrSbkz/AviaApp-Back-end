using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AviaApp.Models.Requests;
using AviaApp.Models.ViewModels;

namespace AviaApp.Services.Contracts;

public interface IFlightService
{
    Task<IList<FlightViewModel>> GetFlightsByDateRangeAsync(DateTime? dateFrom, DateTime? dateTo);

    Task<IList<FlightViewModel>> SearchFlightsAsync(SearchFlightRequest request);

    Task<FlightViewModel> GetFlightByIdAsync(Guid flightId);

    Task<FlightViewModel> AddFlightAsync(AddFlightRequest request);

    Task AddFlightsAsync(IList<AddFlightRequest> request);

    Task<FlightViewModel> UpdateFlightAsync(UpdateFlightRequest request);

    Task DeleteFlightAsync(Guid flightId);

    Task CancelFlightAsync(Guid flightId);

    Task DeleteOutdatedFlightsAsync();
}