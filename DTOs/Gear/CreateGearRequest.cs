using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Packer.Models;

namespace Packer.DTOs;

public class CreateGearRequest
{
    public required Guid Id { get; init; }

    [Required]
    public required string Name { get; init; }

    [Required]
    [Range(typeof(decimal), "0", "79228162514264337593543950335")]
    public decimal? WeightOunces { get; init; }

    [Required]
    [EnumDataType(typeof(GearCategory))]
    [JsonConverter(typeof(JsonStringEnumConverter<GearCategory>))]
    public GearCategory? Category { get; init; }

    public bool IsConsumable { get; init; }
}
