namespace Packer.Models;

public class Gear
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public User User { get; set; } = null!;
    public required string Name { get; set; }
    public decimal WeightOunces { get; set; }
    public GearCategory Category { get; set; }
    public bool IsConsumable { get; set; }
}
