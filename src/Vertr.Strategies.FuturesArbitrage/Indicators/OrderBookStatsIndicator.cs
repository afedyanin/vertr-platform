using Vertr.Common.Contracts;

namespace Vertr.Strategies.FuturesArbitrage.Indicators;

public class OrderBookStatsIndicator
{
    private readonly int _sigmas;

    private readonly BasicStatsCounter _midPriceChangeStats = new BasicStatsCounter();
    private readonly DiffTransformer _midPriceTransformer = new DiffTransformer();

    private readonly BasicStatsCounter _bidsCountStats = new BasicStatsCounter();
    private readonly DiffTransformer _bidsCountTransformer = new DiffTransformer();

    private readonly BasicStatsCounter _bidsValueStats = new BasicStatsCounter();
    private readonly DiffTransformer _bidsValueTransformer = new DiffTransformer();

    private readonly BasicStatsCounter _asksCountStats = new BasicStatsCounter();
    private readonly DiffTransformer _asksCountTransformer = new DiffTransformer();

    private readonly BasicStatsCounter _asksValueStats = new BasicStatsCounter();
    private readonly DiffTransformer _asksValueTransformer = new DiffTransformer();

    public Guid InstrumentId { get; }

    public TradingDirection MidPriceSignal { get; private set; }
    public TradingDirection BidCountSignal { get; private set; }
    public TradingDirection BidValueSignal { get; private set; }
    public TradingDirection AskCountSignal { get; private set; }
    public TradingDirection AskValueSignal { get; private set; }

    public OrderBookStatsIndicator(int sigmas = 1)
    {
        _sigmas = sigmas;
    }

    public void Apply(OrderBook orderBook)
    {
        MidPriceSignal = GetTradingDirection((double)orderBook.MidPrice, _midPriceTransformer, _midPriceChangeStats);
        BidCountSignal = GetTradingDirection(orderBook.Bids.Sum(b => b.QtyLots), _bidsCountTransformer, _bidsCountStats);
        BidValueSignal = GetTradingDirection((double)orderBook.Bids.Sum(b => b.QtyLots * b.Price), _bidsValueTransformer, _bidsValueStats);
        AskCountSignal = GetTradingDirection(orderBook.Asks.Sum(b => b.QtyLots), _asksCountTransformer, _asksCountStats);
        AskValueSignal = GetTradingDirection((double)orderBook.Asks.Sum(b => b.QtyLots * b.Price), _asksValueTransformer, _asksValueStats);
    }

    // Using basic stats to detect order book outliers
    private TradingDirection GetTradingDirection(double value, DiffTransformer diffTransformer, BasicStatsCounter basicStats)
    {
        var change = diffTransformer.Diff(value);

        if (!change.HasValue)
        {
            return TradingDirection.Hold;
        }

        var stats = basicStats.Get();
        var changeValue = change.Value;
        basicStats.Add(changeValue);

        if (!stats.HasValue)
        {
            return TradingDirection.Hold;
        }

        var statsValue = stats.Value;
        var zScore = (changeValue - statsValue.Mean) / statsValue.StdDev;

        return Math.Abs(zScore) >= _sigmas ? change > 0 ? TradingDirection.Buy : TradingDirection.Sell : TradingDirection.Hold;
    }

    public override string? ToString()
    {
        return $"MidPriceSignal={MidPriceSignal} BidCountSignal={BidCountSignal} BidValueSignal={BidValueSignal} AskCountSignal={AskCountSignal} AskValueSignal={AskValueSignal}";
    }
}
