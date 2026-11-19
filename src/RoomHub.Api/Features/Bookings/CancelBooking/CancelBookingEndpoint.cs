using RoomHub.Api.Models;
using RoomHub.Api.Repositories;
using RoomHub.Api.Services;

namespace RoomHub.Api.Features.Bookings.CancelBooking;

public static class CancelBookingEndpoint
{
    public static void Map(RouteGroupBuilder group) => group.MapPost("/{id:guid}/cancel", Handle);

    private static IResult Handle(Guid id, BookingRepository bookings)
    {
        var booking = bookings.Get(id)
                      ?? throw new BusinessException("BookingNotFound", "The booking does not exist.", 404);

        booking.Status = BookingStatus.Cancelled;

        return Results.NoContent();
    }
}
