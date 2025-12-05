using Vertr.TinvestGateway.Contracts.Orders;

namespace Vertr.TinvestGateway.Repositories;

public interface IOrderRequestRepository
{
    public Task Clear();
    public Task<PostOrderRequest?> Get(Guid id);
    public Task<IEnumerable<PostOrderRequest?>> Find(string pattern);
    public Task Save(PostOrderRequest orderRequest, Guid portfolioId);
}