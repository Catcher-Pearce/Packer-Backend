namespace Packer.Models;

public class Trip
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public User User { get; set; } = null!;
    
    public required DateOnly DateStart { get; set; }
    public required DateOnly DateEnd { get; set; }
    
    public required string address { get; set; }
    public required string city { get; set; }
    public required string state { get; set; }
}
