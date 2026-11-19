using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using RoomHub.Api.Common;
using RoomHub.Api.Domain.Bookings;
using RoomHub.Api.Infrastructure.Persistence;

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
        RoomHubDbContext db,
        CancellationToken cancellationToken)
    {
        var b = await db.Bookings
            .AsNoTracking()
            .Where(x => x.Id == id)
            .Select(x => new { x.Id, x.RoomId, x.BookedBy, x.StartUtc, x.EndUtc, x.Attendees, x.Status })
            .FirstOrDefaultAsync(cancellationToken);

        return b is null
            ? BookingErrors.NotFound.ToProblem()
            : TypedResults.Ok(new GetBookingResponse(
                b.Id, b.RoomId, b.BookedBy, b.StartUtc, b.EndUtc, b.Attendees, b.Status.ToString()));
    }
}
