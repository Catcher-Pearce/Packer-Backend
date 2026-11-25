using System.ComponentModel.DataAnnotations;

namespace Packer.DTOs;

public class AddTripGearRequest
{
    public required Guid Id { get; init; }

    [Range(1, int.MaxValue)]
    public required int Quantity { get; init; }
}
