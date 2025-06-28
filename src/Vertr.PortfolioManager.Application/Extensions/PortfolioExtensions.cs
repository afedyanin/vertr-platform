using Vertr.PortfolioManager.Contracts;

namespace Vertr.PortfolioManager.Application.Extensions;

internal static class PortfolioExtensions
{
    public static Portfolio ApplyOperation(this Portfolio portfolio, TradeOperation operation)
    {
        var position = portfolio.GetPosition(operation.InstrumentId);

        if (position == null)
        {
            position = new Position
            {
                InstrumentId = operation.InstrumentId,
                Balance = 0,
            };

            portfolio.Positions.Add(position);
        }

        var _ = operation.OperationType switch
        {
            TradeOperationType.Buy => position.ApplyBuy(operation),
            TradeOperationType.Sell => position.ApplySell(operation),
            TradeOperationType.Input => position.ApplyInput(operation),
            TradeOperationType.Output => position.ApplyOutput(operation),
            TradeOperationType.ServiceFee => position.ApplyFee(operation),
            TradeOperationType.BrokerFee => position.ApplyFee(operation),
            _ => throw new NotImplementedException($"OperationType={operation.OperationType} is not implemented.")
        };

        return portfolio;
    }

    private static Position ApplyBuy(this Position position, TradeOperation operation)
    {
        position.Balance += operation.Quantity;
        return position;
    }

    private static Position ApplySell(this Position position, TradeOperation operation)
    {
        position.Balance -= operation.Quantity;
        return position;
    }

    private static Position ApplyFee(this Position position, TradeOperation operation)
    {
        position.Balance -= operation.Amount ?? decimal.Zero;
        return position;
    }

    private static Position ApplyInput(this Position position, TradeOperation operation)
    {
        position.Balance += operation.Amount ?? decimal.Zero;
        return position;
    }

    private static Position ApplyOutput(this Position position, TradeOperation operation)
    {
        position.Balance -= operation.Amount ?? decimal.Zero;
        return position;
    }

    private static Position? GetPosition(this Portfolio portfolio, Guid instrumentId)
        => portfolio.Positions.FirstOrDefault(p => p.InstrumentId == instrumentId);
}
