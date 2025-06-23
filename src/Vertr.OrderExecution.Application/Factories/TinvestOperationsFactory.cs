using System.Diagnostics;
using Vertr.OrderExecution.Contracts;
using Vertr.OrderExecution.Contracts.Enums;
using Vertr.TinvestGateway.Contracts;

namespace Vertr.OrderExecution.Application.Factories;

internal static class TinvestOperationsFactory
{
    public static TradeOperation[] CreateOperations(
        this PostOrderResponse response,
        PortfolioIdentity portfolioId)
    {
        var opCommission = new TradeOperation
        {
            Id = Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow,
            OperationType = TradeOperationType.BrokerFee,
            BookId = portfolioId.BookId,
            AccountId = portfolioId.AccountId,
            OrderId = response.OrderId,
            Amount = response.ExecutedCommission,
            InstrumentId = Guid.Parse(response.InstrumentId),
        };

        return [opCommission];
    }

    public static TradeOperation[] CreateOperations(
        this OrderState state,
        PortfolioIdentity portfolioId)
    {
        var opTrades = new TradeOperation
        {
            Id = Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow,
            OperationType = state.Direction.ToOperationType(),
            BookId = portfolioId.BookId,
            AccountId = portfolioId.AccountId,
            OrderId = state.OrderId,
            InstrumentId = Guid.Parse(state.InstrumentId),
            Trades = state.OrderStages
        };

        var opCommission = new TradeOperation
        {
            Id = Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow,
            OperationType = TradeOperationType.BrokerFee,
            BookId = portfolioId.BookId,
            AccountId = portfolioId.AccountId,
            OrderId = state.OrderId,
            Amount = state.ExecutedCommission,
            InstrumentId = Guid.Parse(state.InstrumentId),
        };

        return [opTrades, opCommission];
    }

    public static TradeOperation[] CreateOperations(
        this OrderTrades trades,
        PortfolioIdentity portfolioId)
    {
        Debug.Assert(trades.AccountId == portfolioId.AccountId);

        var opTrades = new TradeOperation
        {
            Id = Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow,
            OperationType = trades.Direction.ToOperationType(),
            BookId = portfolioId.BookId,
            AccountId = portfolioId.AccountId,
            OrderId = trades.OrderId,
            InstrumentId = Guid.Parse(trades.InstrumentId),
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
