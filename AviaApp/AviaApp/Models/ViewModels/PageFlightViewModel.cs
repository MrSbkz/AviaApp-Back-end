using System.Collections.Generic;

namespace AviaApp.Models.ViewModels;

public class PageFlightViewModel
{
    public PageInfo PageInfo { get; set; }

    public IList<FlightViewModel> Flights { get; set; }
}