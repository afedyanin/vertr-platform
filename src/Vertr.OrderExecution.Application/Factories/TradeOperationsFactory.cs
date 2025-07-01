using Vertr.OrderExecution.Contracts;
using Vertr.OrderExecution.Contracts.Enums;
using Vertr.PortfolioManager.Contracts;

namespace Vertr.OrderExecution.Application.Factories;

internal static class TradeOperationsFactory
{
    public static TradeOperation[] CreateOperations(
        this PostOrderResponse response,
        Guid instrumentId,
        PortfolioIdentity portfolioIdentity)
    {
        if (response.ExecutedCommission == null)
        {
            return [];
        }

        var opCommission = new TradeOperation
        {
            Id = Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow,
            OperationType = TradeOperationType.BrokerFee,
            AccountId = portfolioIdentity.AccountId,
            SubAccountId = portfolioIdentity.SubAccountId,
            OrderId = response.OrderId,
            Amount = response.ExecutedCommission,
            InstrumentId = instrumentId,
            Price = response.ExecutedCommission,
            Quantity = 1
        };

        return [opCommission];
    }

    public static TradeOperation[] CreateOperations(
        this OrderTrades trades,
        Guid instrumentId,
        PortfolioIdentity portfolioIdentity)
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
                Id = Guid.NewGuid(),
                CreatedAt = DateTime.UtcNow,
                OperationType = trades.Direction.ToOperationType(),
                AccountId = portfolioIdentity.AccountId,
                SubAccountId = portfolioIdentity.SubAccountId,
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

    private static TradeOperationType ToOperationType(this OrderDirection direction)
        => direction switch
        {
            OrderDirection.Unspecified => TradeOperationType.Unspecified,
            OrderDirection.Buy => TradeOperationType.Buy,
            OrderDirection.Sell => TradeOperationType.Sell,
            _ => throw new NotImplementedException(),
        };
}
