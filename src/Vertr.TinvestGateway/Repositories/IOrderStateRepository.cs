using Vertr.TinvestGateway.Contracts.Orders;

namespace Vertr.TinvestGateway.Repositories;

public interface IOrderStateRepository
{
    public Task Clear();
    public Task<IEnumerable<OrderState?>> Find(string pattern);
    public Task Save(OrderState orderState);
}