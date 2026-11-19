using Microsoft.AspNetCore.Http.HttpResults;
using RoomHub.Api.Common;
using RoomHub.Api.Domain.Bookings;

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

    private static async Task<Results<Ok<GetBookingResponse>, ProblemHttpResult>> Handle(
        Guid id,
        IBookingRepository bookings,
        CancellationToken cancellationToken)
    {
        var b = await bookings.GetByIdAsync(id, cancellationToken);

        return b is null
            ? BookingErrors.NotFound.ToProblem()
            : TypedResults.Ok(new GetBookingResponse(
                b.Id, b.RoomId, b.BookedBy, b.StartUtc, b.EndUtc, b.Attendees, b.Status.ToString()));
    }
}
