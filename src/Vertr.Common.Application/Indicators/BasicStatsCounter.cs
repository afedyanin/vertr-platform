using Vertr.Common.Contracts;

namespace Vertr.Common.Application.Indicators;

public class BasicStatsCounter
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
