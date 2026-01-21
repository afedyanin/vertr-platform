namespace Vertr.Common.Application.Indicators;

public class VolumeCounter
{
    public long Count { get; private set; }

    public double Value { get; private set; }

    public void Append(int count, double value)
    {
        Count += count;
        Value += value;
    }

    public void Reset()
    {
        Count = 0;
        Value = 0;
    }
}
