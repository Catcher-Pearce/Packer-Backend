namespace Packer.DTOs;

public record TripFoodResponse(
    Guid Id,
    Guid TripId,
    decimal WeightOunces,
    int Calories,
    int Quantity);
