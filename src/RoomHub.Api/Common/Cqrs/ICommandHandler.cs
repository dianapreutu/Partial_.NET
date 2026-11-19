using RoomHub.Api.Domain.Common;

namespace RoomHub.Api.Common.Cqrs;

// A request that changes the state of the system (marker interface).
public interface ICommand<TResult>
{
}

// One handler per command: loads, calls the domain, saves, returns a Result.
public interface ICommandHandler<in TCommand, TResult>
    where TCommand : ICommand<TResult>
{
    Task<Result<TResult>> Handle(TCommand command, CancellationToken cancellationToken);
}
