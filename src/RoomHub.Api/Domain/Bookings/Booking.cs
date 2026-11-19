using RoomHub.Api.Domain.Common;
using RoomHub.Api.Domain.Rooms;

namespace RoomHub.Api.Domain.Bookings;

public enum BookingStatus
{
    Active,
    Cancelled
}

public sealed class Booking
{
    public static readonly TimeSpan MaxDuration = TimeSpan.FromHours(4);

    public const int MaxActivePerDay = 2;
    public const int BookedByMaxLength = 100;
    public const int LargeRoomCapacity = 10;
    public static readonly TimeSpan MaxDurationLargeRoom = TimeSpan.FromHours(2);
    public static readonly TimeSpan MinCancelNotice = TimeSpan.FromMinutes(60);

    private Booking()
    {
    }

    public Guid Id { get; private set; }
    public Guid RoomId { get; private set; }
    public string BookedBy { get; private set; } = string.Empty;
    public DateTime StartUtc { get; private set; }
    public DateTime EndUtc { get; private set; }
    public int Attendees { get; private set; }
    public BookingStatus Status { get; private set; }

    public static Result<Booking> Create(
        Room room,
        string? bookedBy,
        DateTime startUtc,
        DateTime endUtc,
        int attendees,
        DateTime nowUtc,
        IReadOnlyList<Booking> activeRoomBookings,
        int activeBookingsForDay)
    {
        var error = Validate(room, bookedBy, startUtc, endUtc, attendees, nowUtc);
        if (error is not null)
            return error;

        // Order matters: validation (400), then overlap (409), then the daily limit (409).
        if (activeRoomBookings.Any(b => b.OverlapsWith(startUtc, endUtc)))
            return BookingErrors.RoomOccupied;

        if (activeBookingsForDay >= MaxActivePerDay)
            return BookingErrors.DailyLimitExceeded;

        return new Booking
        {
            Id = Guid.NewGuid(),
            RoomId = room.Id,
            BookedBy = bookedBy!.Trim(),
            StartUtc = startUtc,
            EndUtc = endUtc,
            Attendees = attendees,
            Status = BookingStatus.Active
        };
    }

    private static Error? Validate(
        Room room,
        string? bookedBy,
        DateTime startUtc,
        DateTime endUtc,
        int attendees,
        DateTime nowUtc)
    {
        if (string.IsNullOrWhiteSpace(bookedBy))
            return BookingErrors.InvalidBookedBy;

        if (endUtc <= startUtc)
            return BookingErrors.InvalidRange;

        if (attendees <= 0)
            return BookingErrors.InvalidAttendees;

        if (attendees > room.Capacity)
            return BookingErrors.AttendeesExceedCapacity;

        if (startUtc < nowUtc)
            return BookingErrors.StartInPast;

        if (room.Capacity > LargeRoomCapacity && endUtc - startUtc > MaxDurationLargeRoom)
            return BookingErrors.DurationTooLongForLargeRoom;

        if (endUtc - startUtc > MaxDuration)
            return BookingErrors.TooLong;

        return null;
    }

    // Half-open intervals [start, end): bookings that only touch do not overlap.
    public bool OverlapsWith(DateTime startUtc, DateTime endUtc) =>
        Status == BookingStatus.Active && StartUtc < endUtc && startUtc < EndUtc;

    public Result<Unit> Cancel(DateTime nowUtc)
    {
        if (Status == BookingStatus.Cancelled)
            return BookingErrors.AlreadyCancelled;

        if (StartUtc - nowUtc < MinCancelNotice)
            return BookingErrors.TooLateToCancel;

        Status = BookingStatus.Cancelled;
        return Unit.Value;
    }
}
