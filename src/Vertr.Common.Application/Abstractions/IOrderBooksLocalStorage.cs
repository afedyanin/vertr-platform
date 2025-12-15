using Vertr.Common.Contracts;

namespace Vertr.Common.Application.Abstractions;

public interface IOrderBooksLocalStorage
{
    public OrderBook? GetById(Guid instrumentId);

    public void Update(OrderBook orderBook);
}
