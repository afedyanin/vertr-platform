using System.Diagnostics;
using Vertr.OrderExecution.Contracts;
using Vertr.TinvestGateway.Contracts;
using Vertr.TinvestGateway.Contracts.Enums;

namespace Vertr.OrderExecution.Application.Factories;

public static class TinvestOperationsFactory
{
    public static OrderOperation[] CreateOperations(
        this PostOrderResponse response,
        PortfolioIdentity portfolioId)
    {
        var opCommission = new OrderOperation
        {
            Id = Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow,
            OperationType = Contracts.Enums.OperationType.BrokerFee,
            BookId = portfolioId.BookId,
            AccountId = portfolioId.AccountId,
            OrderId = response.OrderId,
            Amount = response.ExecutedCommission,
            InstrumentId = Guid.Parse(response.InstrumentId),
        };

        return [opCommission];
    }

    public static OrderOperation[] CreateOperations(
        this OrderState state,
        PortfolioIdentity portfolioId)
    {
        var opTrades = new OrderOperation
        {
            Id = Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow,
            OperationType = state.Direction.ToOperationType(),
            BookId = portfolioId.BookId,
            AccountId = portfolioId.AccountId,
            OrderId = state.OrderId,
            InstrumentId = Guid.Parse(state.InstrumentId),
            Trades = state.OrderStages.Convert()
        };

        var opCommission = new OrderOperation
        {
            Id = Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow,
            OperationType = Contracts.Enums.OperationType.BrokerFee,
            BookId = portfolioId.BookId,
            AccountId = portfolioId.AccountId,
            OrderId = state.OrderId,
            Amount = state.ExecutedCommission,
            InstrumentId = Guid.Parse(state.InstrumentId),
        };

        return [opTrades, opCommission];
    }

    public static OrderOperation[] CreateOperations(
        this OrderTrades trades,
        PortfolioIdentity portfolioId)
    {
        Debug.Assert(trades.AccountId == portfolioId.AccountId);

        var opTrades = new OrderOperation
        {
            Id = Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow,
            OperationType = trades.Direction.ToOperationType(),
            BookId = portfolioId.BookId,
            AccountId = portfolioId.AccountId,
            OrderId = trades.OrderId,
            InstrumentId = Guid.Parse(trades.InstrumentId),
            Trades = trades.Trades.Convert()
        };

        return [opTrades];
    }

    private static Contracts.Trade[] Convert(this TinvestGateway.Contracts.Trade[] source)
        => [.. source.Select(Convert)];

    private static Contracts.Trade Convert(this TinvestGateway.Contracts.Trade source)
        => new Contracts.Trade
        {
            Id = source.TradeId,
            ExecutionTime = source.ExecutionTime,
            Price = source.Price,
            Quantity = source.Quantity,
        };

    private static Contracts.Enums.OperationType ToOperationType(this OrderDirection direction)
        => direction switch
        {
            OrderDirection.Unspecified => Contracts.Enums.OperationType.Unspecified,
            OrderDirection.Buy => Contracts.Enums.OperationType.Buy,
            OrderDirection.Sell => Contracts.Enums.OperationType.Sell,
            _ => throw new NotImplementedException(),
        };
}
