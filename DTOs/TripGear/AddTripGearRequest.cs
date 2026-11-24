using System.ComponentModel.DataAnnotations;

namespace Packer.DTOs;

public class AddTripGearRequest
{
    [Range(1, int.MaxValue)]
    public required int Quantity { get; init; }
}
