namespace Vertr.Platform.Common.Sample;

internal class RegisterUserCommandHandler : ICommandHandler<RegisterUserCommand, Guid>
{
    public Task<Guid> Handle(RegisterUserCommand command, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(Guid.NewGuid());
    }
}
