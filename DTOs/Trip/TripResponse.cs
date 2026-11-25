using System.Text.Json.Serialization;

namespace Packer.DTOs;

public record TripResponse(
    Guid Id,
    int UserId,
    DateOnly DateStart,
    DateOnly DateEnd,
    string Address,
    string City,
    string State,
    long Version,
    [property: JsonPropertyName("created_at")] DateTimeOffset CreatedAt,
    [property: JsonPropertyName("updated_at")] DateTimeOffset UpdatedAt);
