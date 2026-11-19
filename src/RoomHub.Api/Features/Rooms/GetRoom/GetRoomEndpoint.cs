using Microsoft.AspNetCore.Http.HttpResults;
using RoomHub.Api.Common;
using RoomHub.Api.Domain.Rooms;

namespace RoomHub.Api.Features.Rooms.GetRoom;

public sealed record GetRoomResponse(Guid Id, string Name, int Capacity);

public static class GetRoomEndpoint
{
    public static void Map(RouteGroupBuilder group) => group.MapGet("/{id:guid}", Handle);

    private static async Task<Results<Ok<GetRoomResponse>, ProblemHttpResult>> Handle(
        Guid id,
        IRoomRepository rooms,
        CancellationToken cancellationToken)
    {
        var room = await rooms.GetByIdAsync(id, cancellationToken);

        return room is null
            ? RoomErrors.NotFound.ToProblem()
            : TypedResults.Ok(new GetRoomResponse(room.Id, room.Name, room.Capacity));
    }
}
