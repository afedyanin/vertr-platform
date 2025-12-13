using Vertr.Common.Contracts;

namespace Vertr.Common.Application.Services;

internal sealed class OrderBookRepository : IOrderBookRepository
{
    private readonly Dictionary<Guid, OrderBook> _books = [];

    public OrderBook? GetById(Guid instrumentId)
    {
        _books.TryGetValue(instrumentId, out var orderBook);
        return orderBook;
    }

    public void Update(OrderBook orderBook)
    {
        _books[orderBook.InstrumentId] = orderBook;
    }

    public Quote? GetMarketQuote(Guid instrumentId)
    {
        if (!_books.TryGetValue(instrumentId, out var orderBook))
        {
            return null;
        }

        return new Quote
        {
            Bid = orderBook.Bids.Max(b => b.Price),
            Ask = orderBook.Asks.Min(a => a.Price),
        };
    }
}

public interface IOrderBookRepository
{
    public OrderBook? GetById(Guid instrumentId);

    public Quote? GetMarketQuote(Guid instrumentId);

    public void Update(OrderBook orderBook);
}