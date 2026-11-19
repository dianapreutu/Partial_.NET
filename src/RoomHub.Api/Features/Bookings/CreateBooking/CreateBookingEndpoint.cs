using RoomHub.Api.Models;
using RoomHub.Api.Repositories;
using RoomHub.Api.Services;

namespace RoomHub.Api.Features.Bookings.CreateBooking;

public sealed record CreateBookingRequest(string? BookedBy, DateTime StartUtc, DateTime EndUtc, int Attendees);

public sealed record CreateBookingResponse(Guid Id, Guid RoomId, string Status);

public static class CreateBookingEndpoint
{
    public static void Map(RouteGroupBuilder group) => group.MapPost("/{roomId:guid}/bookings", Handle);

    private static IResult Handle(
        Guid roomId,
        CreateBookingRequest request,
        RoomRepository rooms,
        BookingRepository bookings)
    {
        var room = rooms.Get(roomId)
                   ?? throw new BusinessException("RoomNotFound", "The room does not exist.", 404);

        if (string.IsNullOrWhiteSpace(request.BookedBy))
            throw new BusinessException("InvalidBookedBy", "BookedBy is required.", 400);

        if (request.EndUtc <= request.StartUtc)
            throw new BusinessException("InvalidRange", "The end must be after the start.", 400);

        if (request.Attendees <= 0)
            throw new BusinessException("InvalidAttendees", "Attendees must be positive.", 400);

        if (request.Attendees > room.Capacity)
            throw new BusinessException("AttendeesExceedCapacity", "The room does not have enough seats.", 400);

        foreach (var existing in bookings.ListForRoom(roomId))
        {
            if (existing.StartUtc <= request.EndUtc && request.StartUtc <= existing.EndUtc)
                throw new BusinessException("RoomOccupied", "The room is occupied in this interval.", 409);
        }

        var booking = new Booking
        {
            Id = Guid.NewGuid(),
            RoomId = roomId,
            BookedBy = request.BookedBy.Trim(),
            StartUtc = request.StartUtc,
            EndUtc = request.EndUtc,
            Attendees = request.Attendees,
            Status = BookingStatus.Active
        };

        bookings.Add(booking);

        return Results.Created(
            $"/api/bookings/{booking.Id}",
            new CreateBookingResponse(booking.Id, booking.RoomId, booking.Status.ToString()));
    }
}
