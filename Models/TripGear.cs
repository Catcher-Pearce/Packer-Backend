namespace Packer.Models;

public class TripGear
{
    public int TripId { get; set; }
    public Trip Trip { get; set; } = null!;
    public int GearId { get; set; }
    public Gear Gear { get; set; } = null!;
    public int Quantity { get; set; }
    public bool IsPacked { get; set; }
}
