using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using Packer.DTOs;
using Packer.DTOs.Gear;
using Packer.Models;
using Packer.Services;

namespace Packer.Controllers;

[ApiController]
[Authorize]
[Route("api/gear")]
public class GearController(GearService gearService, UserManager<User> userManager) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<GearResponse>>> List(
        CancellationToken cancellationToken)
    {
        var account = await userManager.GetUserAsync(User);
        if (account is null)
            return Unauthorized();

        List<GearResponse> gearList = await gearService.GetAsync(account.Id, cancellationToken);
        return Ok(gearList);
    }
    
    [HttpPost]
    public async Task<ActionResult<GearResponse>> Add(
        CreateGearRequest request,
        CancellationToken cancellationToken)
    {
        var account = await userManager.GetUserAsync(User);
        if (account is null)
            return Unauthorized();

        if (request.Id == Guid.Empty)
            return BadRequest(new { message = "Id must be a non-empty UUID." });

        try
        {
            var gear = await gearService.AddAsync(account.Id, request, cancellationToken);
            return StatusCode(StatusCodes.Status201Created, gear);
        }
        catch (DbUpdateException error) when (error.InnerException is PostgresException
            { SqlState: PostgresErrorCodes.UniqueViolation, ConstraintName: "PK_gear" })
        {
            return Conflict(new { message = "This UUID is already in use." });
        }
    }

    [HttpPatch("{id:guid}")]
    public async Task<ActionResult<GearResponse>> Edit(
        Guid id,
        EditGearRequest request,
        CancellationToken cancellationToken)
    {
        var account = await userManager.GetUserAsync(User);
        if (account is null)
            return Unauthorized();

        try
        {
            var gear = await gearService.EditAsync(id, account.Id, request, cancellationToken);
            return gear is null ? NotFound() : Ok(gear);
        }
        catch (ValidationException error)
        {
            return BadRequest(new { message = error.Message });
        }
        catch (DbUpdateConcurrencyException)
        {
            return Conflict(new { message = "The gear changed while it was being edited. Reload it and retry." });
        }
        catch (DbUpdateException error) when (error.InnerException is PostgresException
            { ConstraintName: "PK_sleeping_bag" or "FK_sleeping_bag_gear_GearId" or "CK_sleeping_bag_temperature_order" })
        {
            return Conflict(new { message = "The sleeping bag changed while it was being edited. Reload it and retry." });
        }
    }
    
    [HttpDelete("{id:guid}")]
    public async Task<ActionResult<GearResponse>> Delete(
        Guid id,
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

    [HttpGet("categories")]
    public ActionResult<List<String>> GetCategories()
    {
        return Ok(Enum.GetNames<GearCategory>().ToList());
    }
}
