using RoomHub.Api.Models;
using RoomHub.Api.Repositories;
using RoomHub.Api.Services;

namespace RoomHub.Api.Features.Rooms.CreateRoom;

public sealed record CreateRoomRequest(string? Name, int Capacity);

public sealed record CreateRoomResponse(Guid Id, string Name, int Capacity);

public static class CreateRoomEndpoint
{
    public static void Map(RouteGroupBuilder group) => group.MapPost("/", Handle);

    private static IResult Handle(CreateRoomRequest request, RoomRepository rooms)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
            throw new BusinessException("InvalidName", "Room name is required.", 400);

        if (request.Capacity <= 0)
            throw new BusinessException("InvalidCapacity", "Capacity must be positive.", 400);

        var room = new Room
        {
            Id = Guid.NewGuid(),
            Name = request.Name.Trim(),
            Capacity = request.Capacity
        };

        rooms.Add(room);

        return Results.Created(
            $"/api/rooms/{room.Id}",
            new CreateRoomResponse(room.Id, room.Name, room.Capacity));
    }
}
