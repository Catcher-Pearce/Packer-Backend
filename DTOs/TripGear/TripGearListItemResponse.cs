using System.Text.Json.Serialization;
using Packer.Models;

namespace Packer.DTOs;

public record TripGearListItemResponse(
    int TripId,
    int GearId,
    string Name,
    decimal WeightOunces,
    [property: JsonConverter(typeof(JsonStringEnumConverter<GearCategory>))] GearCategory Category,
    bool IsConsumable,
    int Quantity,
    bool IsPacked);
