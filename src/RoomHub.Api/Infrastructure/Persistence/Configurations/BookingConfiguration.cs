using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RoomHub.Api.Domain.Bookings;

namespace RoomHub.Api.Infrastructure.Persistence.Configurations;

public sealed class BookingConfiguration : IEntityTypeConfiguration<Booking>
{
    public void Configure(EntityTypeBuilder<Booking> builder)
    {
        builder.ToTable("Bookings");
        builder.HasKey(b => b.Id);
        builder.Property(b => b.Id).ValueGeneratedNever();
        builder.Property(b => b.RoomId).IsRequired();
        builder.Property(b => b.BookedBy).IsRequired().HasMaxLength(Booking.BookedByMaxLength);
        builder.Property(b => b.StartUtc).HasConversion<UtcDateTimeConverter>();
        builder.Property(b => b.EndUtc).HasConversion<UtcDateTimeConverter>();
        builder.Property(b => b.Attendees).IsRequired();
        builder.Property(b => b.Status).HasConversion<string>().HasMaxLength(20);

        builder.HasIndex(b => b.RoomId);
        builder.HasIndex(b => new { b.BookedBy, b.StartUtc });
    }
}
