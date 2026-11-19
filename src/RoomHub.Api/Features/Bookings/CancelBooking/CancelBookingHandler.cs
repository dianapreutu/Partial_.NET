using RoomHub.Api.Common.Cqrs;
using RoomHub.Api.Domain.Bookings;
using RoomHub.Api.Domain.Common;

namespace RoomHub.Api.Features.Bookings.CancelBooking;

public sealed record CancelBookingCommand(Guid Id) : ICommand<Unit>;

public sealed class CancelBookingHandler(
    IBookingRepository bookings,
    IUnitOfWork unitOfWork,
    TimeProvider time) : ICommandHandler<CancelBookingCommand, Unit>
{
    public async Task<Result<Unit>> Handle(CancelBookingCommand command, CancellationToken cancellationToken)
    {
        var booking = await bookings.GetByIdAsync(command.Id, cancellationToken);
        if (booking is null)
            return BookingErrors.NotFound;

        var cancelled = booking.Cancel(time.GetUtcNow().UtcDateTime);
        if (cancelled.IsFailure)
            return cancelled.Error;

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return cancelled;
    }
}
