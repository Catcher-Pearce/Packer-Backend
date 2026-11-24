using Microsoft.EntityFrameworkCore;
using Npgsql;
using Packer.Data;
using Packer.Models;

namespace Packer.Repositories;

public class TripGearRepository(PackerDbContext db)
{
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
        int tripId, int gearId, int userId, CancellationToken cancellationToken)
    {
        return OwnedLink(tripId, gearId, userId).ExecuteDeleteAsync(cancellationToken);
    }

    public Task<int> SetPackedAsync(
        int tripId, int gearId, int userId, bool isPacked, CancellationToken cancellationToken)
    {
        return OwnedLink(tripId, gearId, userId)
            .ExecuteUpdateAsync(setters => setters.SetProperty(item => item.IsPacked, isPacked),
                cancellationToken);
    }

    private IQueryable<TripGear> OwnedLink(int tripId, int gearId, int userId)
    {
        return db.TripGear.Where(item => item.TripId == tripId && item.GearId == gearId
            && item.Trip.UserId == userId && item.Gear.UserId == userId);
    }
}
