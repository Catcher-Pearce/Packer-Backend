namespace Packer.DTOs;

public record TripFoodResponse(
    Guid Id,
    Guid TripId,
    string Name,
    decimal WeightOunces,
    int Calories,
    int Quantity);
