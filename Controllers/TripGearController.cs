using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Packer.DTOs;
using Packer.Models;
using Packer.Services;

namespace Packer.Controllers;

[ApiController]
[Authorize]
[Route("api/trips/{tripId:int}/gear")]
public class TripGearController(TripGearService service, UserManager<User> userManager) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<TripGearListItemResponse>>> List(
        int tripId, CancellationToken cancellationToken)
    {
        var account = await userManager.GetUserAsync(User);
        if (account is null)
            return Unauthorized();

        var items = await service.ListAsync(tripId, account.Id, cancellationToken);
        return items is null ? NotFound() : Ok(items);
    }

    [HttpPost("{gearId:int}")]
    public async Task<ActionResult<TripGearResponse>> Add(
        int tripId, int gearId, AddTripGearRequest request, CancellationToken cancellationToken)
    {
        var account = await userManager.GetUserAsync(User);
        if (account is null)
            return Unauthorized();

        var result = await service.AddAsync(tripId, account.Id, gearId, request, cancellationToken);
        if (result.AlreadyExists)
            return Conflict(new { message = "This gear is already on the trip." });

        if (result.Item is null)
            return NotFound();

        return StatusCode(StatusCodes.Status201Created, result.Item);
    }

    [HttpDelete("{gearId:int}")]
    public async Task<IActionResult> Delete(
        int tripId, int gearId, CancellationToken cancellationToken)
    {
        var account = await userManager.GetUserAsync(User);
        if (account is null)
            return Unauthorized();

        return await service.DeleteAsync(tripId, gearId, account.Id, cancellationToken)
            ? NoContent()
            : NotFound();
    }

    [HttpPatch("{gearId:int}/packed")]
    public async Task<IActionResult> SetPacked(
        int tripId, int gearId, SetTripGearPackedRequest request, CancellationToken cancellationToken)
    {
        var account = await userManager.GetUserAsync(User);
        if (account is null)
            return Unauthorized();

        return await service.SetPackedAsync(tripId, gearId, account.Id, request.IsPacked, cancellationToken)
            ? NoContent()
            : NotFound();
    }
}
