using Vertr.Strategies.Application.StrategiesImpl;
using Vertr.Strategies.Contracts;

namespace Vertr.Strategies.Application;

internal static class StrategyFactory
{
    public static StrategyBase Create(StrategyMetadata strategyMetadata)
    {
        ArgumentNullException.ThrowIfNull(strategyMetadata);

        if (strategyMetadata.Type == nameof(RandomWalkStrategy))
        {
            return new RandomWalkStrategy()
            {
                Id = strategyMetadata.Id,
            };
        }

        if (strategyMetadata.Type == nameof(TrendFollowStrategy))
        {
            return new TrendFollowStrategy()
            {
                Id = strategyMetadata.Id,
            };
        }

        throw new ArgumentException($"Invalid strategy type: {strategyMetadata.Type}");
    }
}
