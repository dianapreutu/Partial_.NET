# DECISIONS

Answer **yourself**, in your own words, in 3–5 sentences per question.
Refer to concrete files and classes from your own repo (for example `Features/Bookings/CreateBooking/...`).
An unanswered question gives 0 points.

## 1. A rule you moved
Pick one business rule (R1–R8). In which file did it live in the starter and in which file does it live now? Why there?

Rule R7 (half-open interval overlap check on active bookings only) used to live in Services/BookingService.cs, and now lives in Domain/Bookings/Booking.cs (Booking.OverlapsWith / Booking.Create). Moving it into the Booking aggregate ensures the entity enforces its own invariants independently of persistence or HTTP layers and can be unit-tested directly.

## 2. Result instead of an exception
Why is `Result` a better choice than an exception for "the room is occupied"? What happens in your code when an `Error` is returned?

A room being occupied is a normal case, not a program crash, so returning Result is cleaner and simpler than throwing an exception. In my code, Booking.Create returns Result.Failure with a Conflict error, CreateBookingHandler stops without saving to the database, and BookingEndpoints.cs uses result.Error.ToProblem() to send a 409 Conflict response.

## 3. Who knows about EF Core?
Which classes in your project know about EF Core and which do not? What do you gain and what do you pay with Repository + Unit of Work?

Only RoomHubDbContext, RoomConfiguration, BookingConfiguration, RoomRepository, BookingRepository, the GET endpoints (with .AsNoTracking()), and Program.cs know about EF Core, while Domain/ (Room, Booking), Common/, and the command handlers do not. We gain cleaner code where the domain classes don't depend on the database, and we pay by writing more files and interfaces.

## 4. The flow of a request
Draw in text (ASCII arrows) the path of a `POST /api/rooms/{roomId}/bookings` request through your project, with the **real** names of files and classes, from the endpoint to the database.

_(your answer here)_

## 5. What you rejected from what the AI gave you
Give a concrete example of code or an idea proposed by the AI that you corrected or rejected, and say why.

The AI wanted to use DateTime.UtcNow directly, so I rejected it and asked it to inject TimeProvider in the handlers and pass nowUtc to Booking.Create and Booking.Cancel. I did this because DateTime.UtcNow is hard to test, while TimeProvider makes it easy to test time rules like cancelling 60 minutes before start.

## 6. If you had 30 more minutes
What would you improve and what risk did you leave in the project?

I would start the API and restart it to check that data really stays saved in roomhub.db, and add max length validation (100 chars) in Room.Create and Booking.Create to match RoomConfiguration and BookingConfiguration. The risk left is that if two users book the same room at the exact same time, both requests might pass the check before saving to SQLite.I would start the API and restart it to check that data really stays saved in roomhub.db, and add max length validation (100 chars) in Room.Create and Booking.Create to match RoomConfiguration and BookingConfiguration. The risk left is that if two users book the same room at the exact same time, both requests might pass the check before saving to SQLite.
