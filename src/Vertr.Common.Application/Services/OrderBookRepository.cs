using Vertr.Common.Contracts;

namespace Vertr.Common.Application.Services;

internal sealed class OrderBookRepository : IOrderBookRepository
{
    private readonly Dictionary<Guid, OrderBook> _books = [];

    public async Task<OrderBook?> GetById(Guid instrumentId)
    {
        _books.TryGetValue(instrumentId, out var orderBook);
        return orderBook;
    }

    public void Update(OrderBook orderBook)
    {
        _books[orderBook.InstrumentId] = orderBook;
    }
}

public interface IOrderBookRepository
{
    public Task<OrderBook?> GetById(Guid instrumentId);

    public void Update(OrderBook orderBook);
}