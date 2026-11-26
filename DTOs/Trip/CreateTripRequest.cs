using System.ComponentModel.DataAnnotations;

namespace Packer.DTOs;

public class CreateTripRequest
{
    public required Guid Id { get; init; }

    [Required]
    public required string Name { get; init; }

    public required DateOnly DateStart { get; init; }
    public required DateOnly DateEnd { get; init; }

    [Required]
    public required string Address { get; init; }

    [Required]
    public required string City { get; init; }

    [Required]
    public required string State { get; init; }
}
