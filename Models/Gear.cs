namespace Packer.Models;

public class Gear
{
    public Guid Id { get; set; }
    public int UserId { get; set; }
    public User User { get; set; } = null!;
    public required string Name { get; set; }
    public decimal WeightOunces { get; set; }
    public GearCategory Category { get; set; }
    public bool IsConsumable { get; set; }
    public long Version { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }
}
