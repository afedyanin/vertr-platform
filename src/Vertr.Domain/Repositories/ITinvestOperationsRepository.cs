namespace Vertr.Domain.Repositories;

public interface ITinvestOperationsRepository
{
    public Task<Operation?> GetById(Guid id, CancellationToken cancellationToken = default);

    public Task<IEnumerable<Operation>> Get(
        string accountId,
        DateTime? from = null,
        DateTime? to = null,
        CancellationToken cancellationToken = default);

    public Task<int> Update(Operation operation, CancellationToken cancellationToken = default);

    public Task<int> Insert(Operation operation, CancellationToken cancellationToken = default);

    public Task<int> Delete(Guid operationId, CancellationToken cancellationToken = default);
}
