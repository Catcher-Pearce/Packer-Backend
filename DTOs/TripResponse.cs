namespace Packer.DTOs;

public record TripResponse(
    int Id,
    int UserId,
    DateOnly DateStart,
    DateOnly DateEnd,
    string Address,
    string City,
    string State);
