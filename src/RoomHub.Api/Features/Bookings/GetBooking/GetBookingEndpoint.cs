using RoomHub.Api.Repositories;
using RoomHub.Api.Services;

namespace RoomHub.Api.Features.Bookings.GetBooking;

public sealed record GetBookingResponse(
    Guid Id,
    Guid RoomId,
    string BookedBy,
    DateTime StartUtc,
    DateTime EndUtc,
    int Attendees,
    string Status);

public static class GetBookingEndpoint
{
    public static void Map(RouteGroupBuilder group) => group.MapGet("/{id:guid}", Handle);

    private static IResult Handle(Guid id, BookingRepository bookings)
    {
        var b = bookings.Get(id)
                ?? throw new BusinessException("BookingNotFound", "The booking does not exist.", 404);

        return Results.Ok(new GetBookingResponse(
            b.Id, b.RoomId, b.BookedBy, b.StartUtc, b.EndUtc, b.Attendees, b.Status.ToString()));
    }
}
