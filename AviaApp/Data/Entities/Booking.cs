namespace Data.Entities;

public class Booking
{
    public Guid Id { get; set; }

    public string PhoneNumber { get; set; } = string.Empty;

    public string Country { get; set; } = string.Empty;

    public string City { get; set; } = string.Empty;

    public string BillingAddress { get; set; } = string.Empty;

    public string PostalCode { get; set; } = string.Empty;

    public Guid FlightId { get; set; }

    public string BookedBy { get; set; } = string.Empty;

    public bool IsCanceled { get; set; }

    public DateTime BookingDate { get; set; }

    public decimal Price { get; set; }

    public int CabinClassId { get; set; }

    public Flight? Flight { get; set; }

    public IList<Passenger>? Passengers { get; set; }

    public CabinClass? CabinClass { get; set; }
}