using System.Text.Json.Serialization;

namespace Packer.DTOs;

public record TripGearResponse(
    Guid Id,
    Guid TripId,
    Guid GearId,
    int Quantity,
    bool IsPacked,
    long Version,
    [property: JsonPropertyName("created_at")] DateTimeOffset CreatedAt,
    [property: JsonPropertyName("updated_at")] DateTimeOffset UpdatedAt);
