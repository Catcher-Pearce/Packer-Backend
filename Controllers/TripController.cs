using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Npgsql;
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

        if (request.Id == Guid.Empty)
            return BadRequest(new { message = "Id must be a non-empty UUID." });

        try
        {
            var trip = await tripService.AddAsync(account.Id, request, cancellationToken);
            return CreatedAtAction(nameof(Get), new { id = trip.Id }, trip);
        }
        catch (DbUpdateException error) when (error.InnerException is PostgresException
            { SqlState: PostgresErrorCodes.UniqueViolation, ConstraintName: "PK_trip" })
        {
            return Conflict(new { message = "This UUID is already in use." });
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

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<TripResponse>> Get(Guid id, CancellationToken cancellationToken)
    {
        var account = await userManager.GetUserAsync(User);
        if (account is null)
            return Unauthorized();

        var trip = await tripService.GetAsync(id, account.Id, cancellationToken);
        return trip is null ? NotFound() : Ok(trip);
    }

    [HttpPatch("{id:guid}")]
    public async Task<ActionResult<TripResponse>> Edit(
        Guid id, EditTripRequest request, CancellationToken cancellationToken)
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

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult<TripResponse>> Delete(Guid id, CancellationToken cancellationToken)
    {
        var account = await userManager.GetUserAsync(User);
        if (account is null)
            return Unauthorized();

        var trip = await tripService.DeleteAsync(id, account.Id, cancellationToken);
        return trip is null ? NotFound() : Ok(trip);
    }
}
