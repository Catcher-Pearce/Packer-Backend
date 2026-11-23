using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Packer.DTOs;
using Packer.Models;
using Packer.Services;

namespace Packer.Controllers;

[ApiController]
[Authorize]
[Route("api/gear")]
public class GearController(GearService gearService, UserManager<User> userManager) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<GearResponse>> Add(
        CreateGearRequest request,
        CancellationToken cancellationToken)
    {
        var account = await userManager.GetUserAsync(User);
        if (account is null)
            return Unauthorized();

        var gear = await gearService.AddAsync(account.Id, request, cancellationToken);
        return StatusCode(StatusCodes.Status201Created, gear);
    }

    [HttpPatch("{id:int}")]
    public async Task<ActionResult<GearResponse>> Edit(
        int id,
        EditGearRequest request,
        CancellationToken cancellationToken)
    {
        var account = await userManager.GetUserAsync(User);
        if (account is null)
            return Unauthorized();

        var gear = await gearService.EditAsync(id, account.Id, request, cancellationToken);
        if (gear is null)
            return NotFound();

        return Ok(gear);
    }
    
    [HttpDelete()]
    public async Task<ActionResult<GearResponse>> Delete(
        int id,
        CancellationToken cancellationToken)
    {
        var account = await userManager.GetUserAsync(User);
        if (account is null)
            return Unauthorized();

        var gear = await gearService.DeleteAsync(id, account.Id, cancellationToken);
        if (gear is null)
            return NotFound();

        return Ok(gear);
    }
}
