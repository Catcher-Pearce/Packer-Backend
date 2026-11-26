using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Packer.DTOs;
using Packer.Models;
using Packer.Services;

namespace Packer.Controllers;

[ApiController]
[Authorize]
[Route("api/trips/{tripId:guid}/food")]
public class TripFoodController(TripFoodService service, UserManager<User> userManager) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<TripFoodResponse>>> List(
        Guid tripId, CancellationToken cancellationToken)
    {
        var account = await userManager.GetUserAsync(User);
        if (account is null)
            return Unauthorized();

        var items = await service.ListAsync(tripId, account.Id, cancellationToken);
        return items is null ? NotFound() : Ok(items);
    }

    [HttpPost]
    public async Task<ActionResult<TripFoodResponse>> Add(
        Guid tripId, AddTripFoodRequest request, CancellationToken cancellationToken)
    {
        var account = await userManager.GetUserAsync(User);
        if (account is null)
            return Unauthorized();

        if (request.Id == Guid.Empty)
            return BadRequest(new { message = "Id must be a non-empty UUID." });

        var result = await service.AddAsync(tripId, account.Id, request, cancellationToken);
        if (result.AlreadyExists)
            return Conflict(new { message = "This UUID is already in use." });

        return result.Item is null
            ? NotFound()
            : StatusCode(StatusCodes.Status201Created, result.Item);
    }

    [HttpDelete("{foodId:guid}")]
    public async Task<IActionResult> Delete(
        Guid tripId, Guid foodId, CancellationToken cancellationToken)
    {
        var account = await userManager.GetUserAsync(User);
        if (account is null)
            return Unauthorized();

        return await service.DeleteAsync(tripId, foodId, account.Id, cancellationToken)
            ? NoContent()
            : NotFound();
    }
}
