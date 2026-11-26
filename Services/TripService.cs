using System.ComponentModel.DataAnnotations;
using Packer.DTOs;
using Packer.Models;
using Packer.Repositories;

namespace Packer.Services;

public class TripService(TripRepository tripRepository)
{
    public async Task<TripResponse> AddAsync(
        int userId, CreateTripRequest request, CancellationToken cancellationToken)
    {
        ValidateDates(request.DateStart, request.DateEnd);
        var trip = new Trip
        {
            Id = request.Id,
            UserId = userId,
            Name = request.Name.Trim(),
            DateStart = request.DateStart,
            DateEnd = request.DateEnd,
            address = request.Address.Trim(),
            city = request.City.Trim(),
            state = request.State.Trim()
        };

        await tripRepository.AddAsync(trip, cancellationToken);
        return ToResponse(trip);
    }

    public async Task<IReadOnlyList<TripResponse>> ListAsync(
        int userId, CancellationToken cancellationToken)
    {
        var trips = await tripRepository.ListAsync(userId, cancellationToken);
        return trips.Select(ToResponse).ToList();
    }

    public async Task<TripResponse?> GetAsync(
        Guid id, int userId, CancellationToken cancellationToken)
    {
        var trip = await tripRepository.Find(id, userId, cancellationToken);
        return trip is null ? null : ToResponse(trip);
    }

    public async Task<TripResponse?> EditAsync(
        Guid id, int userId, EditTripRequest request, CancellationToken cancellationToken)
    {
        var trip = await tripRepository.Find(id, userId, cancellationToken);
        if (trip is null)
            return null;

        var dateStart = request.DateStart ?? trip.DateStart;
        var dateEnd = request.DateEnd ?? trip.DateEnd;
        ValidateDates(dateStart, dateEnd);

        if (request.Name is { } name && string.IsNullOrWhiteSpace(name))
            throw new ValidationException("Name must contain a non-whitespace character.");

        var address = ValidateLocation(request.Address, nameof(request.Address)) ?? trip.address;
        var city = ValidateLocation(request.City, nameof(request.City)) ?? trip.city;
        var state = ValidateLocation(request.State, nameof(request.State)) ?? trip.state;

        trip.DateStart = dateStart;
        trip.DateEnd = dateEnd;
        trip.Name = request.Name?.Trim() ?? trip.Name;
        trip.address = address;
        trip.city = city;
        trip.state = state;

        await tripRepository.PatchAsync(cancellationToken);
        return ToResponse(trip);
    }

    public async Task<TripResponse?> DeleteAsync(
        Guid id, int userId, CancellationToken cancellationToken)
    {
        var trip = await tripRepository.Find(id, userId, cancellationToken);
        if (trip is null)
            return null;

        var rowsDeleted = await tripRepository.DeleteAsync(id, userId, cancellationToken);
        return rowsDeleted == 0 ? null : ToResponse(trip);
    }

    private static void ValidateDates(DateOnly dateStart, DateOnly dateEnd)
    {
        if (dateEnd < dateStart)
            throw new ValidationException("The end date must be on or after the start date.");
    }

    private static string? ValidateLocation(string? value, string field)
    {
        if (value is not null && string.IsNullOrWhiteSpace(value))
            throw new ValidationException($"{field} must contain a non-whitespace character.");

        return value?.Trim();
    }

    private static TripResponse ToResponse(Trip trip)
    {
        return new TripResponse(trip.Id, trip.UserId, trip.Name, trip.DateStart, trip.DateEnd,
            trip.address, trip.city, trip.state, trip.Version, trip.CreatedAt, trip.UpdatedAt);
    }
}
