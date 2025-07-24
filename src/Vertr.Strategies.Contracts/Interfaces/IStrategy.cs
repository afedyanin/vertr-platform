using Vertr.MarketData.Contracts;

namespace Vertr.Strategies.Contracts.Interfaces;

public interface IStrategy
{
    public Guid Id { get; }

    public Guid InstrumentId { get; }

    public string AccountId { get; }

    public Guid SubAccountId { get; }

    public long QtyLots { get; }

    public Task HandleMarketData(Candle candle, CancellationToken cancellationToken = default);
}
