using Microsoft.EntityFrameworkCore;
using Packer.Data;
using Packer.Models;

namespace Packer.Repositories;

public class GearRepository(PackerDbContext db)
{
    public async Task AddAsync(Gear gear, CancellationToken cancellationToken)
    {
        db.Gear.Add(gear);
        await db.SaveChangesAsync(cancellationToken);
    }
    public Task<List<Gear>> GetAsync(int userId, CancellationToken cancellationToken)
    {
        return db.Gear
            .AsNoTracking()
            .Include(gear => gear.SleepingBag)
            .Where(gear => gear.UserId == userId)
            .ToListAsync(cancellationToken);
    }

    public Task<Gear?> Find(Guid id, int userId, CancellationToken cancellationToken)
    {
        return db.Gear
            .Include(gear => gear.SleepingBag)
            .SingleOrDefaultAsync(gear => gear.Id == id && gear.UserId == userId, cancellationToken);
    }

    public Task PatchAsync(CancellationToken cancellationToken)
    {
        return db.SaveChangesAsync(cancellationToken);
    }

    public Task<int> DeleteAsync(Guid id, int userId, CancellationToken cancellationToken)
    {
        return db.Gear
            .Where(gear => gear.Id == id && gear.UserId == userId)
            .ExecuteDeleteAsync(cancellationToken);
    }
}
