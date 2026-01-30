namespace Vertr.Strategies.FuturesArbitrage.Indicators;

public class DiffTransformer
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
