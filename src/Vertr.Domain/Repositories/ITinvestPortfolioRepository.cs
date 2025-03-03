namespace Vertr.Domain.Repositories;
public interface ITinvestPortfolioRepository
{
    public Task<PortfolioSnapshot?> GetById(
        Guid id,
        CancellationToken cancellationToken = default);

    public Task<IEnumerable<PortfolioSnapshot>> Get(
        string accountId,
        DateTime? from = null,
        DateTime? to = null,
        CancellationToken cancellationToken = default);

    public Task<PortfolioSnapshot?> GetLast(
        string accountId,
        CancellationToken cancellationToken = default);

    public Task<int> Update(
        PortfolioSnapshot snapshot,
        CancellationToken cancellationToken = default);

    public Task<int> Insert(
        PortfolioSnapshot snapshot,
        CancellationToken cancellationToken = default);

    public Task<int> Delete(
        Guid snapshotId,
        CancellationToken cancellationToken = default);
}
