namespace Vertr.Domain.Repositories;
public interface ITinvestOrdersRepository
{
    public Task<PostOrderResponse?> GetById(
        Guid id,
        CancellationToken cancellationToken = default);

    public Task<IEnumerable<PostOrderResponse>> Get(
        string accountId,
        DateTime? from = null,
        DateTime? to = null,
        CancellationToken cancellationToken = default);

    public Task<PostOrderResponse?> GetLast(
        string accountId,
        CancellationToken cancellationToken = default);

    public Task<PostOrderResponse?> GetByTradingSignal(
        string accountId,
        Guid tradingSignalId,
        CancellationToken cancellationToken= default);

    public Task<int> Update(
        PostOrderResponse order,
        CancellationToken cancellationToken = default);

    public Task<int> Insert(
        PostOrderResponse order,
        CancellationToken cancellationToken = default);

    public Task<int> Delete(
        Guid orderResponseId,
        CancellationToken cancellationToken = default);
}
