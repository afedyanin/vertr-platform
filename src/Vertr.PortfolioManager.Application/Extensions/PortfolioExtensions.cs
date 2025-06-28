using System.Diagnostics;
using Vertr.MarketData.Contracts;
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

        var rubPosition = portfolio.GetPosition(Instrument.RUB);

        if (rubPosition == null)
        {
            rubPosition = new Position
            {
                InstrumentId = Instrument.RUB,
                Balance = 0,
            };

            portfolio.Positions.Add(rubPosition);
        }


        var _ = operation.OperationType switch
        {
            TradeOperationType.Buy => position.ApplyBuy(rubPosition, operation),
            TradeOperationType.Sell => position.ApplySell(rubPosition, operation),
            TradeOperationType.Input => position.ApplyInput(rubPosition, operation),
            TradeOperationType.Output => position.ApplyOutput(rubPosition, operation),
            TradeOperationType.ServiceFee => position.ApplyFee(rubPosition, operation),
            TradeOperationType.BrokerFee => position.ApplyFee(rubPosition, operation),
            _ => throw new NotImplementedException($"OperationType={operation.OperationType} is not implemented.")
        };

        return portfolio;
    }

    private static Position ApplyBuy(
        this Position position,
        Position rubPosition,
        TradeOperation operation)
    {
        rubPosition.Balance -= operation.Amount;
        position.Balance += operation.Quantity;
        return position;
    }

    private static Position ApplySell(
        this Position position,
        Position rubPosition,
        TradeOperation operation)
    {
        rubPosition.Balance += operation.Amount;
        position.Balance -= operation.Quantity;
        return position;
    }

    private static Position ApplyFee(
        this Position position,
        Position rubPosition,
        TradeOperation operation)
    {
        Debug.Assert(position == rubPosition);

        position.Balance -= operation.Amount;
        return position;
    }

    private static Position ApplyInput(
        this Position position,
        Position rubPosition,
        TradeOperation operation)
    {
        Debug.Assert(position == rubPosition);
        position.Balance += operation.Amount;
        return position;
    }

    private static Position ApplyOutput(
        this Position position,
        Position rubPosition,
        TradeOperation operation)
    {
        Debug.Assert(position == rubPosition);
        position.Balance -= operation.Amount;
        return position;
    }

    private static Position? GetPosition(this Portfolio portfolio, Guid instrumentId)
        => portfolio.Positions.FirstOrDefault(p => p.InstrumentId == instrumentId);
}
