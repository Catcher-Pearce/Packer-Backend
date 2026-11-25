using Packer.DTOs;
using Packer.Models;
using Packer.Repositories;

namespace Packer.Services;

public class TripGearService(TripGearRepository repository)
{
    public Task<List<TripGearListItemResponse>?> ListAsync(
        Guid tripId, int userId, CancellationToken cancellationToken)
    {
        return repository.ListAsync(tripId, userId, cancellationToken);
    }

    public async Task<(TripGearResponse? Item, bool AlreadyExists)> AddAsync(
        Guid tripId, int userId, Guid gearId, AddTripGearRequest request, CancellationToken cancellationToken)
    {
        var item = new TripGear
        {
            Id = request.Id,
            TripId = tripId,
            GearId = gearId,
            Quantity = request.Quantity
        };

        var result = await repository.AddAsync(item, userId, cancellationToken);
        return result.Added
            ? (new TripGearResponse(item.Id, item.TripId, item.GearId, item.Quantity, item.IsPacked,
                item.Version, item.CreatedAt, item.UpdatedAt), false)
            : (null, result.AlreadyExists);
    }

    public async Task<bool> DeleteAsync(
        Guid tripId, Guid gearId, int userId, CancellationToken cancellationToken)
    {
        return await repository.DeleteAsync(tripId, gearId, userId, cancellationToken) > 0;
    }

    public async Task<bool> SetPackedAsync(
        Guid tripId, Guid gearId, int userId, bool isPacked, CancellationToken cancellationToken)
    {
        return await repository.SetPackedAsync(tripId, gearId, userId, isPacked, cancellationToken) > 0;
    }
}
