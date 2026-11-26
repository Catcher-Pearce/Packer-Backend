namespace Packer.DTOs;

public class EditTripRequest
{
    public string? Name { get; init; }
    public DateOnly? DateStart { get; init; }
    public DateOnly? DateEnd { get; init; }
    public string? Address { get; init; }
    public string? City { get; init; }
    public string? State { get; init; }
}
