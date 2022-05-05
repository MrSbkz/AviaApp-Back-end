using System.Threading.Tasks;
using AviaApp.Models.Requests;
using Microsoft.AspNetCore.Http;

namespace AviaApp.Services.Contracts;

public interface IBookingService
{
    Task BookFlightAsync(BookFlightRequest request, HttpContext context);
}