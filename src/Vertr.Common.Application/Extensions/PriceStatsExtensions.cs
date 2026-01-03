using Vertr.Common.Contracts;

namespace Vertr.Common.Application.Extensions;

public static class PriceStatsExtensions
{
    public static BasicStats GetStats(this IEnumerable<decimal> values)
    {
        var count = values.Count();

        if (count == 0)
        {
            return new BasicStats
            {
                Count = 0,
                Mean = 0,
                StdDev = 0,
            };
        }

        var avg = values.Average();

        if (count == 1)
        {
            return new BasicStats
            {
                Count = count,
                Mean = (double)avg,
                StdDev = 0,
            };
        }

        var sum = values.Sum(d => (d - avg) * (d - avg));
        var dev = Math.Sqrt((double)sum / (count - 1));

        return new BasicStats
        {
            Count = count,
            Mean = (double)avg,
            StdDev = dev,
        };
    }
}
