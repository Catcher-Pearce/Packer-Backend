namespace Packer.Models;
public class TripFood
{
    public Guid Id { get; set; }
    public Guid TripId { get; set; }
    public Trip Trip { get; set; } = null!;
    public required string Name { get; set; }
    public decimal WeightOunces { get; set; }
    public int Calories { get; set; }
    public int Quantity { get; set; }
}

