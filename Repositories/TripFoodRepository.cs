using Microsoft.EntityFrameworkCore;
using Npgsql;
using Packer.Data;
using Packer.DTOs;
using Packer.Models;

namespace Packer.Repositories;

public class TripFoodRepository(PackerDbContext db)
{
    public Task<List<TripFoodResponse>?> ListAsync(
        Guid tripId, int userId, CancellationToken cancellationToken)
    {
        return db.Trips
            .Where(trip => trip.Id == tripId && trip.UserId == userId)
            .Select(trip => db.TripFood
                .Where(item => item.TripId == trip.Id)
                .OrderBy(item => item.Id)
                .Select(item => new TripFoodResponse(
                    item.Id, item.TripId, item.WeightOunces, item.Calories, item.Quantity))
                .ToList())
            .SingleOrDefaultAsync(cancellationToken);
    }

    public async Task<(bool Added, bool AlreadyExists)> AddAsync(
        TripFood item, int userId, CancellationToken cancellationToken)
    {
        var ownsTrip = await db.Trips.AnyAsync(
            trip => trip.Id == item.TripId && trip.UserId == userId, cancellationToken);
        if (!ownsTrip)
            return (false, false);

        db.TripFood.Add(item);
        try
        {
            await db.SaveChangesAsync(cancellationToken);
            return (true, false);
        }
        catch (DbUpdateException error) when (error.InnerException is PostgresException postgres &&
            postgres is { SqlState: PostgresErrorCodes.UniqueViolation, ConstraintName: "PK_trip_food" }
                or { SqlState: PostgresErrorCodes.ForeignKeyViolation, ConstraintName: "FK_trip_food_trip_TripId" })
        {
            db.Entry(item).State = EntityState.Detached;
            return (false, postgres.SqlState == PostgresErrorCodes.UniqueViolation);
        }
    }

    public Task<int> DeleteAsync(Guid tripId, Guid foodId, int userId, CancellationToken cancellationToken)
    {
        return db.TripFood
            .Where(item => item.Id == foodId && item.TripId == tripId && item.Trip.UserId == userId)
            .ExecuteDeleteAsync(cancellationToken);
    }
}
