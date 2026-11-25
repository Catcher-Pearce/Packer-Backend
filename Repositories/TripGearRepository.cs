using Microsoft.EntityFrameworkCore;
using Npgsql;
using Packer.Data;
using Packer.DTOs;
using Packer.Models;

namespace Packer.Repositories;

public class TripGearRepository(PackerDbContext db)
{
    public Task<List<TripGearListItemResponse>?> ListAsync(
        Guid tripId, int userId, CancellationToken cancellationToken)
    {
        return db.Trips
            .Where(trip => trip.Id == tripId && trip.UserId == userId)
            .Select(trip => db.TripGear
                .Where(item => item.TripId == trip.Id && item.Gear.UserId == userId)
                .OrderBy(item => item.GearId)
                .Select(item => new TripGearListItemResponse(
                    item.Id, item.TripId, item.GearId, item.Gear.Name, item.Gear.WeightOunces,
                    item.Gear.Category, item.Gear.IsConsumable, item.Quantity, item.IsPacked,
                    item.Version, item.CreatedAt, item.UpdatedAt))
                .ToList())
            .SingleOrDefaultAsync(cancellationToken);
    }

    public async Task<(bool Added, bool AlreadyExists)> AddAsync(
        TripGear item, int userId, CancellationToken cancellationToken)
    {
        var ownsTrip = await db.Trips.AnyAsync(
            trip => trip.Id == item.TripId && trip.UserId == userId, cancellationToken);
        if (!ownsTrip)
            return (false, false);

        var ownsGear = await db.Gear.AnyAsync(
            gear => gear.Id == item.GearId && gear.UserId == userId, cancellationToken);
        if (!ownsGear)
            return (false, false);

        db.TripGear.Add(item);
        try
        {
            await db.SaveChangesAsync(cancellationToken);
            return (true, false);
        }
        catch (DbUpdateException error) when (error.InnerException is PostgresException postgres &&
            postgres.SqlState is PostgresErrorCodes.UniqueViolation or PostgresErrorCodes.ForeignKeyViolation)
        {
            db.Entry(item).State = EntityState.Detached;
            return (false, postgres.SqlState == PostgresErrorCodes.UniqueViolation);
        }
    }

    public Task<int> DeleteAsync(
        Guid tripId, Guid gearId, int userId, CancellationToken cancellationToken)
    {
        return OwnedLink(tripId, gearId, userId).ExecuteDeleteAsync(cancellationToken);
    }

    public Task<int> SetPackedAsync(
        Guid tripId, Guid gearId, int userId, bool isPacked, CancellationToken cancellationToken)
    {
        return OwnedLink(tripId, gearId, userId)
            .ExecuteUpdateAsync(setters => setters.SetProperty(item => item.IsPacked, isPacked),
                cancellationToken);
    }

    private IQueryable<TripGear> OwnedLink(Guid tripId, Guid gearId, int userId)
    {
        return db.TripGear.Where(item => item.TripId == tripId && item.GearId == gearId
            && item.Trip.UserId == userId && item.Gear.UserId == userId);
    }
}
