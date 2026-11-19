using RoomHub.Api.Domain.Bookings;

namespace RoomHub.Api.Infrastructure.Persistence.Repositories;

// Temporary in-memory storage (Level B). Replaced by EF Core in Level C.
public sealed class InMemoryBookingRepository : IBookingRepository
{
    private readonly List<Booking> _bookings = [];
    private readonly Lock _lock = new();

    public Task<Booking?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        lock (_lock)
            return Task.FromResult(_bookings.FirstOrDefault(b => b.Id == id));
    }

    public Task<IReadOnlyList<Booking>> ListActiveForRoomAsync(Guid roomId, CancellationToken cancellationToken)
    {
        lock (_lock)
            return Task.FromResult<IReadOnlyList<Booking>>(
                _bookings.Where(b => b.RoomId == roomId && b.Status == BookingStatus.Active).ToList());
    }

    public void Add(Booking booking)
    {
        lock (_lock)
            _bookings.Add(booking);
    }
}
