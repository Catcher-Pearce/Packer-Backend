using System.Text.Json.Serialization;
using Packer.Models;

namespace Packer.DTOs;

public record GearResponse(
    Guid Id,
    int UserId,
    string Name,
    decimal WeightOunces,
    [property: JsonConverter(typeof(JsonStringEnumConverter<GearCategory>))] GearCategory Category,
    long Version,
    [property: JsonPropertyName("created_at")] DateTimeOffset CreatedAt,
    [property: JsonPropertyName("updated_at")] DateTimeOffset UpdatedAt)
{
    public bool IsConsumable { get; init; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public SleepingBagDetailsResponse? SleepingBag { get; init; }
}

public record SleepingBagDetailsResponse(decimal? LowerLimitTempF, decimal? ComfortTempF);
