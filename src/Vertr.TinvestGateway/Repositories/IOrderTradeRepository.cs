using Vertr.TinvestGateway.Models.Orders;

namespace Vertr.TinvestGateway.Repositories;

public interface IOrderTradeRepository
{
    public Task Clear();
    public Task<IEnumerable<OrderTrades?>> Find(string pattern);
    public Task Save(OrderTrades orderTrades);
}