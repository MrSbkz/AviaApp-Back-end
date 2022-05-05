using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AviaApp.Enums;
using AviaApp.Models;
using AviaApp.Models.Dto;
using AviaApp.Services.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AviaApp.Controllers;

[ApiController]
[Route("[controller]")]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    /// <summary>
    /// Returns list of users(Admin, Employee)
    /// </summary>
    /// <remarks>Endpoint is available for "admin" and "employee" roles</remarks>>
    [HttpGet]
    [Route("list")]
    [Authorize(Roles = "admin,employee")]
    [ProducesResponseType(typeof(List<UserDto>), 200)]
    public async Task<IActionResult> GetUsersAsync()
    {
        return Ok(await _userService.GetUsersAsync());
    }

    /// <summary>
    /// Adds new role for user(Admin)
    /// </summary>
    /// <remarks>
    /// Endpoint is available only for "admin" role<br/>
    /// If the role is "banned", the user gets banned and has only "banned" role<br/>
    /// If the role is "user", the user gets unbanned and has only "user" role
    /// </remarks>
    /// <response code="200">The role has been added</response>
    [HttpPost]
    [Route("add-role")]
    [Authorize(Roles = "admin")]
    [ProducesResponseType(typeof(UpdateRoleResponse), 200)]
    [ProducesResponseType(typeof(UpdateRoleResponse), 400)]
    public async Task<IActionResult> AddRoleAsync([FromBody] UpdateRoleModel model)
    {
        var result = await _userService.AddRoleAsync(model);

        return result.Status.Equals(Status.Error, StringComparison.OrdinalIgnoreCase)
            ? BadRequest(result)
            : Ok(result);
    }

    /// <summary>
    /// Delete role of user(Admin)
    /// </summary>
    /// <remarks>
    /// Endpoint is available only for "admin" role<br/>
    /// It is not possible to delete "banned" or "user" role
    /// </remarks>
    /// <response code="200">The role has been deleted</response>
    [HttpPost]
    [Route("delete-role")]
    [Authorize(Roles = "admin")]
    [ProducesResponseType(typeof(UpdateRoleResponse), 200)]
    [ProducesResponseType(typeof(UpdateRoleResponse), 400)]
    public async Task<IActionResult> DeleteRoleAsync([FromBody] UpdateRoleModel model)
    {
        var result = await _userService.DeleteRoleAsync(model);

        return result.Status.Equals(Status.Error, StringComparison.OrdinalIgnoreCase)
            ? BadRequest(result)
            : Ok(result);
    }
}