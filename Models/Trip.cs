namespace Packer.Models;

public class Trip
{
    public Guid Id { get; set; }
    public int UserId { get; set; }
    public User User { get; set; } = null!;
    public required string Name { get; set; }
    
    public required DateOnly DateStart { get; set; }
    public required DateOnly DateEnd { get; set; }
    
    public required string address { get; set; }
    public required string city { get; set; }
    public required string state { get; set; }
    public long Version { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }
}
