using Packer.DTOs;
using Packer.Models;
using Packer.Repositories;

namespace Packer.Services;

public class TripGearService(TripGearRepository repository)
{
    public Task<List<TripGearListItemResponse>?> ListAsync(
        int tripId, int userId, CancellationToken cancellationToken)
    {
        return repository.ListAsync(tripId, userId, cancellationToken);
    }

    public async Task<(TripGearResponse? Item, bool AlreadyExists)> AddAsync(
        int tripId, int userId, int gearId, AddTripGearRequest request, CancellationToken cancellationToken)
    {
        var item = new TripGear
        {
            TripId = tripId,
            GearId = gearId,
            Quantity = request.Quantity
        };

        var result = await repository.AddAsync(item, userId, cancellationToken);
        return result.Added
            ? (new TripGearResponse(item.TripId, item.GearId, item.Quantity, item.IsPacked), false)
            : (null, result.AlreadyExists);
    }

    public async Task<bool> DeleteAsync(
        int tripId, int gearId, int userId, CancellationToken cancellationToken)
    {
        return await repository.DeleteAsync(tripId, gearId, userId, cancellationToken) > 0;
    }

    public async Task<bool> SetPackedAsync(
        int tripId, int gearId, int userId, bool isPacked, CancellationToken cancellationToken)
    {
        return await repository.SetPackedAsync(tripId, gearId, userId, isPacked, cancellationToken) > 0;
    }
}
