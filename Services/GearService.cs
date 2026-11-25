using Packer.DTOs;
using Packer.Models;
using Packer.Repositories;

namespace Packer.Services;

public class GearService(GearRepository repository)
{

    public async Task<List<GearResponse>> GetAsync(int userId, CancellationToken cancellationToken)
    {
        var gearList = await repository.GetAsync(userId, cancellationToken);
        List<GearResponse> gearResponse = gearList.Select(gear => ToResponse(gear)).ToList();
        
        return gearResponse;

    }
    public async Task<GearResponse> AddAsync(
        int userId,
        CreateGearRequest request,
        CancellationToken cancellationToken)
    {
        var gear = new Gear
        {
            Id = request.Id,
            UserId = userId,
            Name = request.Name.Trim(),
            WeightOunces = request.WeightOunces!.Value,
            Category = request.Category!.Value,
            IsConsumable = request.IsConsumable
        };

        await repository.AddAsync(gear, cancellationToken);
        return ToResponse(gear);
    }

    public async Task<GearResponse?> EditAsync(
        Guid id,
        int userId,
        EditGearRequest request,
        CancellationToken cancellationToken)
    {
        var gear = await repository.Find(id, userId, cancellationToken);
        if (gear is null)
            return null;

        if (request.Name is { } name)
            gear.Name = name.Trim();

        if (request.WeightOunces is { } weightOunces)
            gear.WeightOunces = weightOunces;

        if (request.Category is { } category)
            gear.Category = category;

        if (request.IsConsumable is { } isConsumable)
            gear.IsConsumable = isConsumable;

        await repository.PatchAsync(cancellationToken);
        return ToResponse(gear);
    }

    public async Task<GearResponse?> DeleteAsync(
        Guid id,
        int userId,
        CancellationToken cancellationToken)
    {
        var gear = await repository.Find(id, userId, cancellationToken);
        if (gear is null) {
            return null;
        }
        
        
        await repository.DeleteAsync(id, userId, cancellationToken);
        return ToResponse(gear);
    }

    private static GearResponse ToResponse(Gear gear)
    {
        return new GearResponse(gear.Id, gear.UserId, gear.Name, gear.WeightOunces, gear.Category,
            gear.Version, gear.CreatedAt, gear.UpdatedAt)
        {
            IsConsumable = gear.IsConsumable
        };
    }
}
