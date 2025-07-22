namespace Vertr.Platform.Common;

public interface ICommandHandler<in TCommand> where TCommand : ICommand
{
    public Task Handle(TCommand command, CancellationToken cancellationToken = default);
}

public interface ICommandHandler<in TCommand, TResponse> where TCommand : ICommand<TResponse>
{
    public Task<TResponse> Handle(TCommand command, CancellationToken cancellationToken = default);
}
