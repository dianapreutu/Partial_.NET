namespace RoomHub.Api.Domain.Common;

// Saves everything that changed during the request. The DbContext implements it.
public interface IUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
