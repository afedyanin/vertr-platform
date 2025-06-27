using Vertr.MarketData.Contracts;
using Vertr.PortfolioManager.Contracts;

namespace Vertr.PortfolioManager.Application.Extensions;

internal static class PortfolioExtensions
{
    public static async Task<Portfolio> ApplyOperation(this Portfolio portfolio, TradeOperation operation)
    {
        var applyTask = operation.OperationType switch
        {
            TradeOperationType.Buy => portfolio.ApplyBuy(operation),
            // TODO: Implement this
            TradeOperationType.Unspecified => throw new NotImplementedException(),
            TradeOperationType.Input => throw new NotImplementedException(),
            TradeOperationType.Output => throw new NotImplementedException(),
            TradeOperationType.ServiceFee => throw new NotImplementedException(),
            TradeOperationType.BrokerFee => throw new NotImplementedException(),
            TradeOperationType.Sell => throw new NotImplementedException(),
            _ => throw new NotImplementedException()
        };

        await applyTask;

        return portfolio;
    }

    private static Task ApplyBuy(this Portfolio portfolio, TradeOperation operation)
    {
        // TODO: Implement this
        return Task.CompletedTask;
    }

    private static Position? GetPosition(this Portfolio portfolio, InstrumentIdentity instrumentIdentity)
        => portfolio.Positions.FirstOrDefault(p => p.InstrumentIdentity == instrumentIdentity);
}
