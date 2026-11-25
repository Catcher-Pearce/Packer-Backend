using System.Text.Json.Serialization;
using Packer.Models;

namespace Packer.DTOs;

public record TripGearListItemResponse(
    Guid Id,
    Guid TripId,
    Guid GearId,
    string Name,
    decimal WeightOunces,
    [property: JsonConverter(typeof(JsonStringEnumConverter<GearCategory>))] GearCategory Category,
    bool IsConsumable,
    int Quantity,
    bool IsPacked,
    long Version,
    [property: JsonPropertyName("created_at")] DateTimeOffset CreatedAt,
    [property: JsonPropertyName("updated_at")] DateTimeOffset UpdatedAt);
