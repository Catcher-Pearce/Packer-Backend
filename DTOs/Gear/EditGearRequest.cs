using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Packer.Models;

namespace Packer.DTOs;

public class EditGearRequest : IValidatableObject
{
    public string? Name { get; init; }

    [Range(typeof(decimal), "0", "79228162514264337593543950335")]
    public decimal? WeightOunces { get; init; }

    [EnumDataType(typeof(GearCategory))]
    [JsonConverter(typeof(JsonStringEnumConverter<GearCategory>))]
    public GearCategory? Category { get; init; }

    public bool? IsConsumable { get; init; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (Name is not null && string.IsNullOrWhiteSpace(Name))
            yield return new ValidationResult("Name must contain a non-whitespace character.", [nameof(Name)]);
    }
}
