using System.Diagnostics;
using Vertr.MarketData.Contracts;
using Vertr.OrderExecution.Contracts;
using Vertr.OrderExecution.Contracts.Enums;
using Vertr.PortfolioManager.Contracts;

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
            AccountId = portfolioIdentity.AccountId,
            BookId = portfolioIdentity.BookId,
            OrderId = response.OrderId,
            Amount = response.ExecutedCommission,
            ClassCode = instrumentIdentity.ClassCode,
            Ticker = instrumentIdentity.Ticker,
        };

        return [opCommission];
    }

    public static TradeOperation[] CreateOperations(
        this OrderTrades trades,
        InstrumentIdentity instrumentIdentity,
        PortfolioIdentity portfolioIdentity)
    {
        Debug.Assert(trades.AccountId == portfolioIdentity.AccountId);

        var opTrades = new List<TradeOperation>();

        foreach (var trade in trades.Trades)
        {
            var opTrade = new TradeOperation
            {
                Id = Guid.NewGuid(),
                CreatedAt = DateTime.UtcNow,
                OperationType = trades.Direction.ToOperationType(),
                AccountId = portfolioIdentity.AccountId,
                BookId = portfolioIdentity.BookId,
                OrderId = trades.OrderId,
                ClassCode = instrumentIdentity.ClassCode,
                Ticker = instrumentIdentity.Ticker,
                Price = trade.Price,
                Quantity = trade.Quantity,
                ExecutionTime = trade.ExecutionTime,
                TradeId = trade.TradeId,
            };
        }

        return [.. opTrades];
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
