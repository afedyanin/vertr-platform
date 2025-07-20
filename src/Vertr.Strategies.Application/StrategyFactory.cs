using Vertr.PortfolioManager.Contracts;
using Vertr.Strategies.Application.StrategiesImpl;
using Vertr.Strategies.Contracts;

namespace Vertr.Strategies.Application;

internal static class StrategyFactory
{
    public static StrategyBase Create(StrategyMetadata strategyMetadata, IServiceProvider serviceProvider)
    {
        ArgumentNullException.ThrowIfNull(strategyMetadata);

        if (strategyMetadata.Type == nameof(RandomWalkStrategy))
        {
            return new RandomWalkStrategy(serviceProvider)
            {
                Id = strategyMetadata.Id,
                PortfolioIdentity = new PortfolioIdentity(strategyMetadata.AccountId, strategyMetadata.SubAccountId),
                InstrumentId = strategyMetadata.InstrumentId,
                QtyLots = strategyMetadata.QtyLots,
            };
        }

        if (strategyMetadata.Type == nameof(TrendFollowStrategy))
        {
            return new TrendFollowStrategy(serviceProvider)
            {
                Id = strategyMetadata.Id,
                PortfolioIdentity = new PortfolioIdentity(strategyMetadata.AccountId, strategyMetadata.SubAccountId),
                InstrumentId = strategyMetadata.InstrumentId,
                QtyLots = strategyMetadata.QtyLots,
            };
        }

        throw new ArgumentException($"Invalid strategy type: {strategyMetadata.Type}");
    }
}
