namespace Vertr.Platform.Common;

public interface IQueryHandler<in TQuery, TResponse> where TQuery : IQuery<TResponse>
{
    public Task<TResponse> Handle(TQuery command, CancellationToken cancellationToken = default);
}
