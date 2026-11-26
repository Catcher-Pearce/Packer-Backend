using Packer.DTOs;
using Packer.Models;
using Packer.Repositories;

namespace Packer.Services;

public class TripFoodService(TripFoodRepository repository)
{
    public Task<List<TripFoodResponse>?> ListAsync(
        Guid tripId, int userId, CancellationToken cancellationToken)
    {
        return repository.ListAsync(tripId, userId, cancellationToken);
    }

    public async Task<(TripFoodResponse? Item, bool AlreadyExists)> AddAsync(
        Guid tripId, int userId, AddTripFoodRequest request, CancellationToken cancellationToken)
    {
        var item = new TripFood
        {
            Id = request.Id,
            TripId = tripId,
            WeightOunces = request.WeightOunces,
            Calories = request.Calories,
            Quantity = request.Quantity
        };

        var result = await repository.AddAsync(item, userId, cancellationToken);
        return result.Added
            ? (new TripFoodResponse(item.Id, item.TripId, item.WeightOunces, item.Calories, item.Quantity), false)
            : (null, result.AlreadyExists);
    }

    public async Task<bool> DeleteAsync(
        Guid tripId, Guid foodId, int userId, CancellationToken cancellationToken)
    {
        return await repository.DeleteAsync(tripId, foodId, userId, cancellationToken) > 0;
    }
}
