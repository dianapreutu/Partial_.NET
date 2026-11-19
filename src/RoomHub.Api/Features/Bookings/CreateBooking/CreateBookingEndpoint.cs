using Microsoft.AspNetCore.Http.HttpResults;
using RoomHub.Api.Common;
using RoomHub.Api.Common.Cqrs;

namespace RoomHub.Api.Features.Bookings.CreateBooking;

public sealed record CreateBookingRequest(string? BookedBy, DateTime StartUtc, DateTime EndUtc, int Attendees);

public static class CreateBookingEndpoint
{
    public static void Map(RouteGroupBuilder group) => group.MapPost("/{roomId:guid}/bookings", Handle);

    private static async Task<Results<Created<CreateBookingResponse>, ProblemHttpResult>> Handle(
        Guid roomId,
        CreateBookingRequest request,
        ICommandHandler<CreateBookingCommand, CreateBookingResponse> handler,
        CancellationToken cancellationToken)
    {
        var command = new CreateBookingCommand(
            roomId, request.BookedBy, request.StartUtc, request.EndUtc, request.Attendees);

        var result = await handler.Handle(command, cancellationToken);

        return result.IsSuccess
            ? TypedResults.Created($"/api/bookings/{result.Value.Id}", result.Value)
            : result.Error.ToProblem();
    }
}
