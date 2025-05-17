using Vertr.OrderExecution.Contracts;
using Vertr.TinvestGateway.Contracts;

namespace Vertr.OrderExecution.Application.Factories;

public static class TinvestOperationsFactory
{
    public static OrderOperation[] CreateOperations(
        this PostOrderResponse response,
        string accountId,
        Guid bookId)
    {
        var opCommission = new OrderOperation
        {
            Id = Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow,
            OperationType = Contracts.OperationType.BrokerFee,
            BookId = bookId,
            AccountId = accountId,
            OrderId = response.OrderId,
            Amount = response.ExecutedCommission,
            InstrumentId = Guid.Parse(response.InstrumentId),
        };

        return [opCommission];
    }

    public static OrderOperation[] CreateOperations(
        this OrderState state,
        string? accountId,
        Guid? bookId)
    {
        var opTrades = new OrderOperation
        {
            Id = Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow,
            OperationType = state.Direction.ToOperationType(),
            BookId = bookId,
            AccountId = accountId,
            OrderId = state.OrderId,
            InstrumentId = Guid.Parse(state.InstrumentId),
            Trades = state.OrderStages.Convert()
        };

        var opCommission = new OrderOperation
        {
            Id = Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow,
            OperationType = Contracts.OperationType.BrokerFee,
            BookId = bookId,
            AccountId = accountId,
            OrderId = state.OrderId,
            Amount = state.ExecutedCommission,
            InstrumentId = Guid.Parse(state.InstrumentId),
        };

        return [opTrades, opCommission];
    }

    public static OrderOperation[] CreateOperations(
        this OrderTrades trades,
        Guid? bookId)
    {
        var opTrades = new OrderOperation
        {
            Id = Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow,
            OperationType = trades.Direction.ToOperationType(),
            BookId = bookId,
            AccountId = trades.AccountId,
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

    private static Contracts.OperationType ToOperationType(this OrderDirection direction)
        => direction switch
        {
            OrderDirection.Unspecified => Contracts.OperationType.Unspecified,
            OrderDirection.Buy => Contracts.OperationType.Buy,
            OrderDirection.Sell => Contracts.OperationType.Sell,
            _ => throw new NotImplementedException(),
        };
}
