using System.ComponentModel.DataAnnotations;

namespace Packer.DTOs;

public class AddTripFoodRequest
{
    public required Guid Id { get; init; }

    [Required]
    public required string Name { get; init; }

    [Range(typeof(decimal), "0", "79228162514264337593543950335")]
    public required decimal WeightOunces { get; init; }

    [Range(0, int.MaxValue)]
    public required int Calories { get; init; }

    [Range(1, int.MaxValue)]
    public required int Quantity { get; init; }
}
