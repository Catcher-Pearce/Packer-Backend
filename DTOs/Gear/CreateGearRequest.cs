using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Packer.Models;

namespace Packer.DTOs.Gear;

public class CreateGearRequest : IValidatableObject
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
    public SleepingBagDetailsRequest? SleepingBag { get; init; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (SleepingBag is not null && Category != GearCategory.SleepingBag)
        {
            yield return new ValidationResult(
                "Sleeping bag details require the SleepingBag category.", [nameof(SleepingBag)]);
        }
    }
}
