using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AviaApp.Models.Dto;
using AviaApp.Models.Requests;
using AviaApp.Services.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AviaApp.Controllers;

[ApiController]
[Route("[controller]")]
public class CityController : ControllerBase
{
    private readonly ICityService _cityService;

    public CityController(ICityService cityService)
    {
        _cityService = cityService;
    }

    /// <summary>
    /// Returns list of cities by country Id(Admin, Employee, User)
    /// </summary>
    /// <remarks>Endpoint is available for authorized users</remarks>>
    /// <param name="countryId">Country Id</param>
    [HttpGet]
    [Route("list/{countryId:guid}")]
    [Authorize(Roles = "admin,employee,user")]
    [ProducesResponseType(typeof(List<CityDto>), 200)]
    public async Task<IActionResult> GetCitiesAsync(Guid countryId)
    {
        return Ok(await _cityService.GetCitiesAsync(countryId));
    }

    /// <summary>
    /// Returns city by city Id(Admin, Employee, User)
    /// </summary>
    /// <remarks>Endpoint is available for authorized users</remarks>>
    /// <param name="cityId">City Id</param>
    [HttpGet]
    [Route("{cityId:guid}")]
    [Authorize(Roles = "admin,employee,user")]
    [ProducesResponseType(typeof(CityDto), 200)]
    public async Task<IActionResult> GetCityByIdAsync(Guid cityId)
    {
        try
        {
            return Ok(await _cityService.GetCityByIdAsync(cityId));
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    /// <summary>
    /// Adds city(Admin, Employee)
    /// </summary>
    /// <remarks>
    /// Endpoint is available for "admin" and "employee" roles<br/>
    /// The field "name" is unique for country group
    /// </remarks>>
    [HttpPost]
    [Authorize(Roles = "admin,employee")]
    [ProducesResponseType(typeof(CityDto), 200)]
    public async Task<IActionResult> AddCityAsync(AddCityRequest request)
    {
        try
        {
            return Ok(await _cityService.AddCityAsync(request));
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    /// <summary>
    /// Updates city name(Admin, Employee)
    /// </summary>
    /// <remarks>
    /// Endpoint is available for "admin" and "employee" roles<br/>
    /// The field "name" is unique for country group
    /// </remarks>>
    [HttpPut]
    [Authorize(Roles = "admin,employee")]
    public async Task<IActionResult> UpdateCityNameAsync([FromBody] UpdateCityRequest request)
    {
        try
        {
            await _cityService.UpdateCityNameAsync(request);
            return Ok("The city has been updated successfully");
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    /// <summary>
    /// Deletes city(Admin, Employee)
    /// </summary>
    /// <remarks>
    /// Endpoint is available for "admin" and "employee" roles<br/>
    /// Related objects will also be deleted
    /// </remarks>>
    [HttpDelete("{cityId:guid}")]
    [Authorize(Roles = "admin,employee")]
    public async Task<IActionResult> DeleteCityAsync(Guid cityId)
    {
        try
        {
            await _cityService.DeleteCityAsync(cityId);
            return Ok("The city has been deleted");
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
}