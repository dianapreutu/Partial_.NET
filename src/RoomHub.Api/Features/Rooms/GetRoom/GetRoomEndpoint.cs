using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using RoomHub.Api.Common;
using RoomHub.Api.Domain.Rooms;
using RoomHub.Api.Infrastructure.Persistence;

namespace RoomHub.Api.Features.Rooms.GetRoom;

public sealed record GetRoomResponse(Guid Id, string Name, int Capacity);

public static class GetRoomEndpoint
{
    public static void Map(RouteGroupBuilder group) => group.MapGet("/{id:guid}", Handle);

    private static async Task<Results<Ok<GetRoomResponse>, ProblemHttpResult>> Handle(
        Guid id,
        RoomHubDbContext db,
        CancellationToken cancellationToken)
    {
        var room = await db.Rooms
            .AsNoTracking()
            .Where(r => r.Id == id)
            .Select(r => new GetRoomResponse(r.Id, r.Name, r.Capacity))
            .FirstOrDefaultAsync(cancellationToken);

        return room is null
            ? RoomErrors.NotFound.ToProblem()
            : TypedResults.Ok(room);
    }
}
