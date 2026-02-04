using Vertr.Common.Contracts;

namespace Vertr.Strategies.FuturesArbitrage.Models;

public class OrderBookStatsInfo
{
    private readonly int _sigmas;

    private readonly BasicStatsCounter _midPriceChangeStats = new BasicStatsCounter();
    private readonly DiffTransformer _midPriceTransformer = new DiffTransformer();

    public Guid InstrumentId { get; }

    public TradingDirection MidPriceSignal { get; private set; }

    public OrderBookStatsInfo(int sigmas = 1)
    {
        _sigmas = sigmas;
    }

    public void Apply(OrderBook orderBook)
    {
        MidPriceSignal = GetTradingDirection((double)orderBook.MidPrice, _midPriceTransformer, _midPriceChangeStats);
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
}

// TODO: Make it private?
internal sealed class DiffTransformer
{
    public double? Previous { get; private set; }

    public DiffTransformer(double? initialValue = null)
    {
        Previous = initialValue;
    }

    public double? Diff(double value)
    {
        if (Previous == null)
        {
            Previous = value;
            return null;
        }

        if (Previous == 0)
        {
            Previous = value;
            return value;
        }

        var res = (value - Previous) / Previous;
        Previous = value;
        return res;
    }
}

// TODO: Make it private?
internal sealed class BasicStatsCounter
{
    private readonly LinkedList<double> _values = new LinkedList<double>();

    private readonly int _count;

    private double _avg;

    private double _std;

    private bool IsValid => _values.Count == _count;

    public double? Last => _values.Last?.Value;

    public BasicStatsCounter(int count = 10)
    {
        if (count < 2)
        {
            throw new ArgumentException($"Invalid count={count}. Count must be >= 2.");
        }

        _count = count;
    }

    public void Add(double nextValue)
    {
        if (_values.Count >= _count)
        {
            _values.RemoveFirst();
        }

        _values.AddLast(nextValue);

        if (!IsValid)
        {
            return;
        }

        _avg = _values.Average();
        var sum = _values.Sum(d => (d - _avg) * (d - _avg));
        _std = Math.Sqrt(sum / (_count - 1));
    }

    public BasicStats? Get() => IsValid ?
        new BasicStats
        {
            Mean = _avg,
            StdDev = _std,
            Count = _values.Count,
        }
        : null;

    public void Reset()
    {
        _values.Clear();
        _avg = 0;
        _std = 0;
    }
}
