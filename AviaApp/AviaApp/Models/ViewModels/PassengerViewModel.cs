using System;

namespace AviaApp.Models.ViewModels;

public class PassengerViewModel
{
    public Guid Id { get; set; }

    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public bool IsCanceled { get; set; }

    public DateTime? CancelDate { get; set; }
}