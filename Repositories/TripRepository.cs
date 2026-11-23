using Microsoft.EntityFrameworkCore;
using Packer.Data;
using Packer.Models;

namespace Packer.Repositories;

public class TripRepository(PackerDbContext db)
{
    public async Task AddAsync(Trip trip, CancellationToken cancellationToken)
    {
        db.Trips.Add(trip);
        await db.SaveChangesAsync(cancellationToken);
    }

    public Task<List<Trip>> ListAsync(int userId, CancellationToken cancellationToken)
    {
        return db.Trips.AsNoTracking()
            .Where(trip => trip.UserId == userId)
            .OrderBy(trip => trip.Id)
            .ToListAsync(cancellationToken);
    }

    public Task<Trip?> Find(int id, int userId, CancellationToken cancellationToken)
    {
        return db.Trips.SingleOrDefaultAsync(
            trip => trip.Id == id && trip.UserId == userId, cancellationToken);
    }

    public Task PatchAsync(CancellationToken cancellationToken)
    {
        return db.SaveChangesAsync(cancellationToken);
    }

    public Task<int> DeleteAsync(int id, int userId, CancellationToken cancellationToken)
    {
        return db.Trips
            .Where(trip => trip.Id == id && trip.UserId == userId)
            .ExecuteDeleteAsync(cancellationToken);
    }
}
