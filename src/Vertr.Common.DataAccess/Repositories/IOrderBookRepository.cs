using Vertr.Common.Contracts;

namespace Vertr.Common.DataAccess.Repositories;

public interface IOrderBookRepository
{
    public Task Clear();
    public Task<IEnumerable<OrderBook>> GetAll();
    public Task<OrderBook?> Get(Guid instrumentId);
    public Task Save(OrderBook orderBook);
}
