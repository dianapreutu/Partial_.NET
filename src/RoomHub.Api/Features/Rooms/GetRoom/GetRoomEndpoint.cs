using RoomHub.Api.Repositories;
using RoomHub.Api.Services;

namespace RoomHub.Api.Features.Rooms.GetRoom;

public sealed record GetRoomResponse(Guid Id, string Name, int Capacity);

public static class GetRoomEndpoint
{
    public static void Map(RouteGroupBuilder group) => group.MapGet("/{id:guid}", Handle);

    private static IResult Handle(Guid id, RoomRepository rooms)
    {
        var room = rooms.Get(id)
                   ?? throw new BusinessException("RoomNotFound", "The room does not exist.", 404);

        return Results.Ok(new GetRoomResponse(room.Id, room.Name, room.Capacity));
    }
}
