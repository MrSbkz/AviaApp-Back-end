using System;
using System.Threading.Tasks;
using AviaApp.Models.Requests;
using AviaApp.Models.ViewModels;
using AviaApp.Services.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AviaApp.Controllers;

[ApiController]
[Route("[controller]")]
public class BookingController : ControllerBase
{
    private readonly IBookingService _bookingService;

    public BookingController(IBookingService bookingService)
    {
        _bookingService = bookingService;
    }

    /// <summary>
    /// Books a flight(Admin, Employee, User)
    /// </summary>
    /// <remarks>
    /// Endpoint is available for admin and employee roles
    /// </remarks>
    [HttpPost]
    [Authorize(Roles = "admin,employee,user")]
    public async Task<IActionResult> AddFlightAsync([FromBody] BookFlightRequest request)
    {
        try
        {
            await _bookingService.BookFlightAsync(request);
            return Ok();
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
}