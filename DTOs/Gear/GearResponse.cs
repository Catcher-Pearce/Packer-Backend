using System.Text.Json.Serialization;
using Packer.Models;

namespace Packer.DTOs;

public record GearResponse(
    int Id,
    int UserId,
    string Name,
    decimal WeightOunces,
    [property: JsonConverter(typeof(JsonStringEnumConverter<GearCategory>))] GearCategory Category)
{
    public bool IsConsumable { get; init; }
}
