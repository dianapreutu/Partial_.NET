using RoomHub.Api.Features.Bookings;
using RoomHub.Api.Features.Rooms;
using RoomHub.Api.Middleware;
using RoomHub.Api.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<RoomRepository>();
builder.Services.AddSingleton<BookingRepository>();

var app = builder.Build();

app.UseMiddleware<ExceptionMiddleware>();

app.MapGet("/", () => "RoomHub API is running.");
app.MapRooms();
app.MapBookings();

app.Run();
