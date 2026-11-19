using RoomHub.Api.Common.Cqrs;
using RoomHub.Api.Domain.Bookings;
using RoomHub.Api.Domain.Common;
using RoomHub.Api.Domain.Rooms;

namespace RoomHub.Api.Features.Bookings.CreateBooking;

public sealed record CreateBookingCommand(
    Guid RoomId,
    string? BookedBy,
    DateTime StartUtc,
    DateTime EndUtc,
    int Attendees) : ICommand<CreateBookingResponse>;

public sealed record CreateBookingResponse(Guid Id, Guid RoomId, string Status);

public sealed class CreateBookingHandler(
    IRoomRepository rooms,
    IBookingRepository bookings,
    TimeProvider time) : ICommandHandler<CreateBookingCommand, CreateBookingResponse>
{
    public async Task<Result<CreateBookingResponse>> Handle(
        CreateBookingCommand command,
        CancellationToken cancellationToken)
    {
        // 404 for a missing room comes before any validation.
        var room = await rooms.GetByIdAsync(command.RoomId, cancellationToken);
        if (room is null)
            return RoomErrors.NotFound;

        var created = Booking.Create(
            room,
            command.BookedBy,
            command.StartUtc,
            command.EndUtc,
            command.Attendees,
            time.GetUtcNow().UtcDateTime);

        if (created.IsFailure)
            return created.Error;

        var active = await bookings.ListActiveForRoomAsync(room.Id, cancellationToken);
        if (active.Any(b => b.OverlapsWith(command.StartUtc, command.EndUtc)))
            return BookingErrors.RoomOccupied;

        var booking = created.Value;
        bookings.Add(booking);

        return new CreateBookingResponse(booking.Id, booking.RoomId, booking.Status.ToString());
    }
}
