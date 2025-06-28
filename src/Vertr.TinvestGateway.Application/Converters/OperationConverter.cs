using Vertr.PortfolioManager.Contracts;

namespace Vertr.TinvestGateway.Application.Converters;

internal static class OperationConverter
{
    public static TradeOperation[]? Convert(
        this Tinkoff.InvestApi.V1.OperationsResponse source,
        string accountId)
    {
        if (source == null)
        {
            return null;
        }

        var operations = new List<TradeOperation>();
        foreach (var operation in source.Operations)
        {
            if (operation.Trades.Count != 0)
            {
                foreach (var trade in operation.Trades)
                {
                    var opTrade = operation.ConvertTrade(accountId, trade);

                    if (opTrade != null)
                    {
                        operations.Add(opTrade);
                    }
                }
            }
            else
            {
                var converted = operation.Convert(accountId);

                if (converted != null)
                {
                    operations.Add(converted);
                }
            }
        }

        return [.. operations];
    }

    public static TradeOperation? Convert(
        this Tinkoff.InvestApi.V1.Operation source,
        string accountId)
    {
        if (source == null)
        {
            return null;
        }

        var res = new TradeOperation
        {
            Id = Guid.Parse(source.Id),
            CreatedAt = source.Date.ToDateTime(),
            OperationType = source.OperationType.Convert(),
            AccountId = accountId,
            SubAccountId = Guid.Empty,
            InstrumentId = Guid.Parse(source.InstrumentUid),
            Amount = source.Payment.Convert(),
            Price = source.Price.Convert(),
            Quantity = source.Quantity,
            Message = source.Type
        };

        return res;
    }

    public static TradeOperation? ConvertTrade(
        this Tinkoff.InvestApi.V1.Operation source,
        string accountId,
        Tinkoff.InvestApi.V1.OperationTrade operationTrade)
    {
        if (source == null)
        {
            return null;
        }

        var res = new TradeOperation
        {
            Id = Guid.Parse(source.Id),
            CreatedAt = source.Date.ToDateTime(),
            OperationType = source.OperationType.Convert(),
            AccountId = accountId,
            SubAccountId = Guid.Empty,
            InstrumentId = Guid.Parse(source.InstrumentUid),
            Price = operationTrade.Price.Convert(),
            Quantity = operationTrade.Quantity,
            TradeId = operationTrade.TradeId,
            ExecutionTime = operationTrade.DateTime.ToDateTime(),
            Message = source.Type
        };

        return res;
    }
}
