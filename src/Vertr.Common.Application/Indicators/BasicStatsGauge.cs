namespace Vertr.Common.Application.Indicators;

public class BasicStatsGauge
{
    private readonly LinkedList<double> _values = new LinkedList<double>();

    private readonly int _maxCount;

    public string Name { get; }

    public int Count => _values.Count;

    public double Last => _values.Last?.Value ?? 0;

    public double Mean { get; private set; }

    public double StdDev { get; private set; }

    public BasicStatsGauge(string name = "", int maxCount = 100)
    {
        Name = name;
        _maxCount = maxCount;
    }

    public void Add(double nextValue)
    {
        if (_values.Count >= _maxCount)
        {
            _values.RemoveFirst();
        }

        _values.AddLast(nextValue);

        CalculateStats();
    }

    public void Reset()
    {
        _values.Clear();
    }

    private void CalculateStats()
    {
        var count = _values.Count;

        if (count == 0)
        {
            return;
        }

        Mean = _values.Average();

        if (count < 2)
        {
            return;
        }

        var sum = _values.Sum(d => (d - Mean) * (d - Mean));
        StdDev = Math.Sqrt(sum / (count - 1));
    }
}
