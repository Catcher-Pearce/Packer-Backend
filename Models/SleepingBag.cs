namespace Packer.Models;

public class SleepingBag
{
    public Guid GearId { get; set; }
    public Gear Gear { get; set; } = null!;
    public decimal? LowerLimitTempF { get; set; }
    public decimal? ComfortTempF { get; set; }
}
