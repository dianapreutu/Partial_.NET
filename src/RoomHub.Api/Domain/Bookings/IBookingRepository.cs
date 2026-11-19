namespace RoomHub.Api.Domain.Bookings;

public interface IBookingRepository
{
    Task<Booking?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<IReadOnlyList<Booking>> ListActiveForRoomAsync(Guid roomId, CancellationToken cancellationToken);
    Task<int> CountActiveForBookedByOnDayAsync(string bookedBy, DateTime dayUtc, CancellationToken cancellationToken);
    void Add(Booking booking);
}
