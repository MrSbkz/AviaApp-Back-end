using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AviaApp.Models.Requests;
using AviaApp.Models.ViewModels;
using AviaApp.Services.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AviaApp.Controllers;

[ApiController]
[Route("[controller]")]
public class FlightController : ControllerBase
{
    private readonly IFlightService _flightService;

    public FlightController(IFlightService flightService)
    {
        _flightService = flightService;
    }

    /// <summary>
    /// Returns list of flights by date range(Admin, Employee)
    /// </summary>
    /// <remarks>
    /// Endpoint is available for admin and employee roles<br/>
    /// "dateFrom" and "dateTo" are nullable
    /// </remarks>
    [HttpGet]
    [Route("list/by-date-range")]
    [Authorize(Roles = "admin,employee")]
    [ProducesResponseType(typeof(List<FlightViewModel>), 200)]
    public async Task<IActionResult> GetFlightsByDateRangeAsync([FromQuery] DateTime? dateFrom,
        [FromQuery] DateTime? dateTo)
    {
        try
        {
            return Ok(await _flightService.GetFlightsByDateRangeAsync(dateFrom, dateTo));
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    /// <summary>
    /// Returns list of flights by search parameters
    /// </summary>
    /// <remarks>
    /// "countryIdFrom", "countryIdTo" and "flightDateTime" are required fields<br/>
    /// It is not possible to get canceled flights<br/>
    /// </remarks>
    [HttpPost]
    [Route("list/search")]
    [ProducesResponseType(typeof(List<FlightViewModel>), 200)]
    public async Task<IActionResult> GetFlightsByDateAsync([FromBody] SearchFlightRequest request)
    {
        try
        {
            return Ok(await _flightService.SearchFlightsAsync(request));
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    /// <summary>
    /// Returns flight by Id
    /// </summary>
    [HttpGet]
    [Route("{flightId:guid}")]
    [ProducesResponseType(typeof(FlightViewModel), 200)]
    public async Task<IActionResult> GetFlightByIdAsync(Guid flightId)
    {
        try
        {
            return Ok(await _flightService.GetFlightByIdAsync(flightId));
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    /// <summary>
    /// Adds flight(Admin, Employee)
    /// </summary>
    /// <remarks>
    /// Endpoint is available for admin and employee roles
    /// </remarks>
    [HttpPost]
    [Authorize(Roles = "admin,employee")]
    [ProducesResponseType(typeof(FlightViewModel), 200)]
    public async Task<IActionResult> AddFlightAsync([FromBody] AddFlightRequest request)
    {
        try
        {
            return Ok(await _flightService.AddFlightAsync(request));
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    /// <summary>
    /// Adds flights(Auto Job)
    /// </summary>
    /// <remarks>
    /// Endpoint is available for admin and employee roles
    /// </remarks>
    [HttpPost]
    [Route("add-list")]
    [Authorize(Roles = "auto-job")]
    public async Task<IActionResult> AddFlightsAsync([FromBody] IList<AddFlightRequest> request)
    {
        try
        {
            await _flightService.AddFlightsAsync(request);
            return Ok();
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    /// <summary>
    /// Updates flight(Admin, Employee)
    /// </summary>
    /// <remarks>
    /// Endpoint is available for admin and employee roles<br/>
    /// You can change only departure date, arrival date and airplane<br/>
    /// If some field has null value this one will not be changed<br/>
    /// You are not able to update canceled flight
    /// </remarks>
    [HttpPut]
    [Authorize(Roles = "admin,employee")]
    [ProducesResponseType(typeof(FlightViewModel), 200)]
    public async Task<IActionResult> UpdateFlightAsync([FromBody] UpdateFlightRequest request)
    {
        try
        {
            return Ok(await _flightService.UpdateFlightAsync(request));
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    /// <summary>
    /// Deletes flight(Admin)
    /// </summary>
    /// <remarks>
    /// Endpoint is available for admin role<br/>
    /// </remarks>
    [HttpDelete]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> DeleteFlightAsync(Guid flightId)
    {
        try
        {
            await _flightService.DeleteFlightAsync(flightId);
            return Ok("The flight has been deleted successfully");
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    /// <summary>
    /// Cancels flight(Admin)
    /// </summary>
    /// <remarks>
    /// Endpoint is available for admin role<br/>
    /// </remarks>
    [HttpDelete]
    [Route("cancel")]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> CancelFlightAsync(Guid flightId)
    {
        try
        {
            await _flightService.CancelFlightAsync(flightId);
            return Ok();
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    /// <summary>
    /// Deletes outdated flights(Auto Job)
    /// </summary>
    /// <remarks>
    /// The endpoint is available for auto job role<br/>
    /// </remarks>
    [HttpDelete]
    [Route("outdated")]
    [Authorize(Roles = "auto-job")]
    public async Task<IActionResult> DeleteOutdatedFlightsAsync()
    {
        try
        {
            await _flightService.DeleteOutdatedFlightsAsync();
            return Ok();
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
}