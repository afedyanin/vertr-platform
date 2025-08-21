using Vertr.OrderExecution.Contracts;
using Vertr.OrderExecution.Contracts.Enums;
using Vertr.PortfolioManager.Contracts;

namespace Vertr.OrderExecution.Application.Factories;

internal static class TradeOperationsFactory
{
    public static TradeOperation[] CreateFromOrderResponse(
        PostOrderResponse response,
        Guid instrumentId,
        Guid portfolioId,
        string accountId,
        DateTime createdtAt)
    {
        if (response.ExecutedCommission == null)
        {
            return [];
        }

        var opCommission = new TradeOperation
        {
            CreatedAt = createdtAt,
            OperationType = TradeOperationType.BrokerFee,
            AccountId = accountId,
            PortfolioId = portfolioId,
            OrderId = response.OrderId,
            Amount = response.ExecutedCommission,
            InstrumentId = instrumentId,
            Price = response.ExecutedCommission,
            Quantity = 1
        };

        return [opCommission];
    }

    public static TradeOperation[] CreateFromOrderTrades(
        OrderTrades trades,
        Guid instrumentId,
        Guid portfolioId,
        string accountId)
    {
        var opTrades = new List<TradeOperation>();

        foreach (var trade in trades.Trades)
        {
            if (trade.Price == null)
            {
                continue;
            }

            var opTrade = new TradeOperation
            {
                CreatedAt = trades.CreatedAt,
                OperationType = trades.Direction.ToOperationType(),
                AccountId = accountId,
                PortfolioId = portfolioId,
                OrderId = trades.OrderId,
                InstrumentId = instrumentId,
                Price = trade.Price,
                Quantity = trade.Quantity,
                TradeId = trade.TradeId,
                Amount = new Money(trade.Price.Value * trade.Quantity, trade.Price.Currency),
            };

            opTrades.Add(opTrade);
        }

        return [.. opTrades];
    }

    public static TradeOperation[] CreateFromOrderState(
        OrderState state,
        Guid instrumentId,
        Guid portfolioId,
        string accountId)
    {
        var opTrades = new List<TradeOperation>();

        foreach (var trade in state.OrderStages)
        {
            if (trade.Price == null)
            {
                continue;
            }

            var opTrade = new TradeOperation
            {
                CreatedAt = state.CreatedAt,
                OperationType = state.Direction.ToOperationType(),
                AccountId = accountId,
                PortfolioId = portfolioId,
                OrderId = state.OrderId,
                InstrumentId = instrumentId,
                Price = trade.Price,
                Quantity = trade.Quantity,
                TradeId = trade.TradeId,
                Amount = new Money(trade.Price.Value * trade.Quantity, trade.Price.Currency),
            };

            opTrades.Add(opTrade);
        }

        if (state.ExecutedCommission != null)
        {
            var opCommission = new TradeOperation
            {
                CreatedAt = state.CreatedAt,
                OperationType = TradeOperationType.BrokerFee,
                PortfolioId = portfolioId,
                AccountId = accountId,
                OrderId = state.OrderId,
                Amount = state.ExecutedCommission,
                InstrumentId = instrumentId,
                Price = state.ExecutedCommission,
                Quantity = 1
            };

            opTrades.Add(opCommission);
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
