using System;
using System.Collections.Generic;

namespace AviaApp.Models.Requests;

public class BookFlightRequest
{
    public string PhoneNumber { get; set; }

    public string Country { get; set; }

    public string City { get; set; }

    public string BillingAddress { get; set; }

    public string PostalCode { get; set; }

    public int CabinClassId { get; set; }

    public Guid FlightId { get; set; }

    public IList<PassengerRequest> Passengers { get; set; }
}