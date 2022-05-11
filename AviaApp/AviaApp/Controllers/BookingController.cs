using System;
using System.Threading.Tasks;
using AviaApp.Helpers;
using AviaApp.Models.Requests;
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
    /// Endpoint is available for admin, employee and user roles
    /// </remarks>
    [HttpPost]
    [Authorize(Roles = "admin,employee,user")]
    public async Task<IActionResult> AddFlightAsync([FromBody] BookFlightRequest request)
    {
        try
        {
            var email = HttpContextHelper.GetEmailFromContext(HttpContext);
            await _bookingService.BookFlightAsync(request, email);
            return Ok();
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    /// <summary>
    /// Cancels a booking(Admin, Employee, User)
    /// </summary>
    /// <remarks>
    /// Endpoint is available for admin, employee and user roles
    /// </remarks>
    [HttpDelete]
    [Route("cancel/{bookingId:guid}")]
    [Authorize(Roles = "admin,employee,user")]
    public async Task<IActionResult> CancelBookingAsync(Guid bookingId)
    {
        try
        {
            await _bookingService.CancelBookingAsync(bookingId);
            return Ok();
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    /// <summary>
    /// Cancels a booking for passenger(Admin, Employee, User)
    /// </summary>
    /// <remarks>
    /// Endpoint is available for admin, employee and user roles
    /// </remarks>
    [HttpDelete]
    [Route("cancel/passenger/{passengerId:guid}")]
    [Authorize(Roles = "admin,employee,user")]
    public async Task<IActionResult> CancelBookingForPassengerAsync(Guid passengerId)
    {
        try
        {
            await _bookingService.CancelBookingForPassengerAsync(passengerId);
            return Ok();
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
}