using Vertr.TinvestGateway.Contracts.Orders;

namespace Vertr.TinvestGateway.Repositories;

public interface IOrderResponseRepository
{
    public Task Clear();
    public Task<PostOrderResponse?> Get(Guid id);
    public Task<IEnumerable<PostOrderResponse?>> Find(string pattern);
    public Task Save(PostOrderResponse orderResponse);
}