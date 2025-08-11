namespace Vertr.Platform.Common.Mediator;
public interface IMediator
{
    public Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default);

    public Task Send(IRequest request, CancellationToken cancellationToken = default);
}
