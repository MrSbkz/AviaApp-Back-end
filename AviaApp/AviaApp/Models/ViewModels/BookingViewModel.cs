using System;
using System.Collections.Generic;

namespace AviaApp.Models.ViewModels;

public class BookingViewModel
{
    public Guid Id { get; set; }

    public string PhoneNumber { get; set; } = string.Empty;

    public string Country { get; set; } = string.Empty;

    public string City { get; set; } = string.Empty;

    public string BillingAddress { get; set; } = string.Empty;

    public string PostalCode { get; set; } = string.Empty;

    public bool IsCanceled { get; set; }

    public DateTime BookingDate { get; set; }

    public decimal Price { get; set; }

    public FlightViewModel Flight { get; set; }

    public IList<PassengerViewModel> Passengers { get; set; }

    public CabinClassViewModel CabinClass { get; set; }
}