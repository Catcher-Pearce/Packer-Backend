namespace Packer.Models;

public class TripGear
{
    public Guid Id { get; set; }
    public Guid TripId { get; set; }
    public Trip Trip { get; set; } = null!;
    public Guid GearId { get; set; }
    public Gear Gear { get; set; } = null!;
    public int Quantity { get; set; }
    public bool IsPacked { get; set; }
    public long Version { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }
}
