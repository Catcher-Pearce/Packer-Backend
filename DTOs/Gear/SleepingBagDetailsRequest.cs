using System.ComponentModel.DataAnnotations;

namespace Packer.DTOs.Gear;

public class SleepingBagDetailsRequest : IValidatableObject
{
    private readonly decimal? lowerLimitTempF;
    private readonly decimal? comfortTempF;

    public decimal? LowerLimitTempF
    {
        get => lowerLimitTempF;
        init
        {
            lowerLimitTempF = value;
            HasLowerLimitTempF = true;
        }
    }

    public decimal? ComfortTempF
    {
        get => comfortTempF;
        init
        {
            comfortTempF = value;
            HasComfortTempF = true;
        }
    }

    internal bool HasLowerLimitTempF { get; private init; }
    internal bool HasComfortTempF { get; private init; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (LowerLimitTempF > ComfortTempF)
        {
            yield return new ValidationResult(
                "The lower limit must be at or below the comfort temperature.",
                [nameof(LowerLimitTempF), nameof(ComfortTempF)]);
        }
    }
}
