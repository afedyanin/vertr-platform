using Vertr.Strategies.Application.StrategiesImpl;
using Vertr.Strategies.Contracts;
using Vertr.Strategies.Contracts.Interfaces;

namespace Vertr.Strategies.Application;

internal static class StrategyFactory
{
    public static IStrategy Create(StrategyMetadata strategyMetadata, IServiceProvider serviceProvider)
    {
        ArgumentNullException.ThrowIfNull(strategyMetadata);

        if (strategyMetadata.Type == StrategyType.RandomWalk)
        {
            return new RandomWalkStrategy(serviceProvider)
            {
                Id = strategyMetadata.Id,
                BacktestId = strategyMetadata.BacktestId,
                SubAccountId = strategyMetadata.SubAccountId,
                InstrumentId = strategyMetadata.InstrumentId,
                QtyLots = strategyMetadata.QtyLots,
            };
        }

        if (strategyMetadata.Type == StrategyType.TrendFollow)
        {
            return new TrendFollowStrategy(serviceProvider)
            {
                Id = strategyMetadata.Id,
                BacktestId= strategyMetadata.BacktestId,
                SubAccountId = strategyMetadata.SubAccountId,
                InstrumentId = strategyMetadata.InstrumentId,
                QtyLots = strategyMetadata.QtyLots,
            };
        }

        throw new ArgumentException($"Invalid strategy type: {strategyMetadata.Type}");
    }
}
