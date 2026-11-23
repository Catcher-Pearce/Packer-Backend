using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Packer.DTOs;
using Packer.Models;
using Packer.Services;

namespace Packer.Controllers;

[ApiController]
[Authorize]
[Route("api/trips")]
public class TripController(TripService tripService, UserManager<User> userManager) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<TripResponse>> Add(
        CreateTripRequest request, CancellationToken cancellationToken)
    {
        var account = await userManager.GetUserAsync(User);
        if (account is null)
            return Unauthorized();

        try
        {
            var trip = await tripService.AddAsync(account.Id, request, cancellationToken);
            return CreatedAtAction(nameof(Get), new { id = trip.Id }, trip);
        }
        catch (ValidationException error)
        {
            return BadRequest(new { message = error.Message });
        }
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<TripResponse>>> List(CancellationToken cancellationToken)
    {
        var account = await userManager.GetUserAsync(User);
        if (account is null)
            return Unauthorized();

        return Ok(await tripService.ListAsync(account.Id, cancellationToken));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<TripResponse>> Get(int id, CancellationToken cancellationToken)
    {
        var account = await userManager.GetUserAsync(User);
        if (account is null)
            return Unauthorized();

        var trip = await tripService.GetAsync(id, account.Id, cancellationToken);
        return trip is null ? NotFound() : Ok(trip);
    }

    [HttpPatch("{id:int}")]
    public async Task<ActionResult<TripResponse>> Edit(
        int id, EditTripRequest request, CancellationToken cancellationToken)
    {
        var account = await userManager.GetUserAsync(User);
        if (account is null)
            return Unauthorized();

        try
        {
            var trip = await tripService.EditAsync(id, account.Id, request, cancellationToken);
            return trip is null ? NotFound() : Ok(trip);
        }
        catch (ValidationException error)
        {
            return BadRequest(new { message = error.Message });
        }
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult<TripResponse>> Delete(int id, CancellationToken cancellationToken)
    {
        var account = await userManager.GetUserAsync(User);
        if (account is null)
            return Unauthorized();

        var trip = await tripService.DeleteAsync(id, account.Id, cancellationToken);
        return trip is null ? NotFound() : Ok(trip);
    }
}
