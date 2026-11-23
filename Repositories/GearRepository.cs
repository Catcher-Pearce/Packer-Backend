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

    public Task<Gear?> Find(int id, int userId, CancellationToken cancellationToken)
    {
        return db.Gear.SingleOrDefaultAsync(
            gear => gear.Id == id && gear.UserId == userId, cancellationToken);
    }

    public Task PatchAsync(CancellationToken cancellationToken)
    {
        return db.SaveChangesAsync(cancellationToken);
    }

    public Task<int> DeleteAsync(int id, int userId, CancellationToken cancellationToken)
    {
        return db.Gear
            .Where(gear => gear.Id == id && gear.UserId == userId)
            .ExecuteDeleteAsync(cancellationToken);
    }
}
