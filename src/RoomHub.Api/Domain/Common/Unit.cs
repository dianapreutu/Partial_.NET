namespace RoomHub.Api.Domain.Common;

// "Nothing", for operations that return no data: Result<Unit>.
public sealed class Unit
{
    public static readonly Unit Value = new();

    private Unit()
    {
    }
}
