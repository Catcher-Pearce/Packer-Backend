using System.ComponentModel.DataAnnotations;
using Packer.DTOs;
using Packer.DTOs.Gear;
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

        if (request.SleepingBag is { } details)
        {
            gear.SleepingBag = new SleepingBag
            {
                GearId = gear.Id,
                LowerLimitTempF = details.LowerLimitTempF,
                ComfortTempF = details.ComfortTempF
            };
        }

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

        ApplySleepingBagEdit(request, gear);

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

    private static void ApplySleepingBagEdit(EditGearRequest request, Gear gear)
    {
        if (request.SleepingBag is { } details)
        {
            if ((request.Category ?? gear.Category) != GearCategory.SleepingBag)
                throw new ValidationException("Sleeping bag details require the SleepingBag category.");

            if (details.HasLowerLimitTempF || details.HasComfortTempF)
            {
                var bag = gear.SleepingBag;
                var lowerLimit = details.HasLowerLimitTempF ? details.LowerLimitTempF : bag?.LowerLimitTempF;
                var comfort = details.HasComfortTempF ? details.ComfortTempF : bag?.ComfortTempF;
                if (lowerLimit > comfort)
                    throw new ValidationException("The lower limit must be at or below the comfort temperature.");

                bag ??= new SleepingBag { GearId = gear.Id };
                bag.LowerLimitTempF = lowerLimit;
                bag.ComfortTempF = comfort;
                gear.SleepingBag = bag;
            }
        }
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
            IsConsumable = gear.IsConsumable,
            SleepingBag = gear.Category == GearCategory.SleepingBag && gear.SleepingBag is { } details
                ? new SleepingBagDetailsResponse(details.LowerLimitTempF, details.ComfortTempF)
                : null
        };
    }
}
