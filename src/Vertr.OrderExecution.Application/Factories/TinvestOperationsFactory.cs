using System.Diagnostics;
using Vertr.MarketData.Contracts;
using Vertr.OrderExecution.Contracts;
using Vertr.OrderExecution.Contracts.Enums;

namespace Vertr.OrderExecution.Application.Factories;

internal static class TinvestOperationsFactory
{
    public static TradeOperation[] CreateOperations(
        this PostOrderResponse response,
        InstrumentIdentity instrumentIdentity,
        PortfolioIdentity portfolioIdentity)
    {
        var opCommission = new TradeOperation
        {
            Id = Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow,
            OperationType = TradeOperationType.BrokerFee,
            PortfolioIdentity = portfolioIdentity,
            OrderId = response.OrderId,
            Amount = response.ExecutedCommission,
            InstrumentIdentity = instrumentIdentity,
        };

        return [opCommission];
    }

    public static TradeOperation[] CreateOperations(
        this OrderTrades trades,
        InstrumentIdentity instrumentIdentity,
        PortfolioIdentity portfolioIdentity)
    {
        Debug.Assert(trades.AccountId == portfolioIdentity.AccountId);

        var opTrades = new TradeOperation
        {
            Id = Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow,
            OperationType = trades.Direction.ToOperationType(),
            PortfolioIdentity = portfolioIdentity,
            OrderId = trades.OrderId,
            InstrumentIdentity = instrumentIdentity,
            Trades = trades.Trades
        };

        return [opTrades];
    }

    private static TradeOperationType ToOperationType(this OrderDirection direction)
        => direction switch
        {
            OrderDirection.Unspecified => TradeOperationType.Unspecified,
            OrderDirection.Buy => TradeOperationType.Buy,
            OrderDirection.Sell => TradeOperationType.Sell,
            _ => throw new NotImplementedException(),
        };
}
