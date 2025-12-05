using Vertr.TinvestGateway.Contracts.Portfolios;

namespace Vertr.TinvestGateway.Converters;

internal static class OperationConverter
{
    public static TradeOperation[] Convert(
        this Tinkoff.InvestApi.V1.GetOperationsByCursorResponse source,
        string accountId)
    {
        if (source == null)
        {
            return [];
        }

        var operations = new List<TradeOperation>();
        foreach (var operation in source.Items)
        {
            if (IsBySell(operation))
            {
                foreach (var trade in operation.TradesInfo.Trades)
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


    private static TradeOperation? Convert(
        this Tinkoff.InvestApi.V1.OperationItem source,
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
            OperationType = source.Type.Convert(),
            AccountId = accountId,
            PortfolioId = Guid.Empty,
            InstrumentId = Guid.Parse(source.InstrumentUid),
            Amount = source.Payment.Convert(),
            Price = source.Price.Convert(),
            Quantity = source.Quantity,
        };

        return res;
    }

    private static TradeOperation? ConvertTrade(
        this Tinkoff.InvestApi.V1.OperationItem source,
        string accountId,
        Tinkoff.InvestApi.V1.OperationItemTrade operationTrade)
    {
        if (source == null)
        {
            return null;
        }

        var price = operationTrade.Price.Convert();
        var priceAamount = price.Value * operationTrade.Quantity;

        var res = new TradeOperation
        {
            Id = Guid.Parse(source.Id),
            CreatedAt = operationTrade.Date.ToDateTime(),
            OperationType = source.Type.Convert(),
            AccountId = accountId,
            PortfolioId = Guid.Empty,
            InstrumentId = Guid.Parse(source.InstrumentUid),
            Price = operationTrade.Price.Convert(),
            Quantity = operationTrade.Quantity,
            Amount = new Money(priceAamount, price.Currency),
            TradeId = operationTrade.Num,
        };

        return res;
    }

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
            if (IsBySell(operation))
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
            PortfolioId = Guid.Empty,
            InstrumentId = Guid.Parse(source.InstrumentUid),
            Amount = source.Payment.Convert(),
            Price = source.Price.Convert(),
            Quantity = source.Quantity,
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

        var price = operationTrade.Price.Convert();
        var priceAamount = price.Value * operationTrade.Quantity;

        var res = new TradeOperation
        {
            Id = Guid.Parse(source.Id),
            CreatedAt = operationTrade.DateTime.ToDateTime(),
            OperationType = source.OperationType.Convert(),
            AccountId = accountId,
            PortfolioId = Guid.Empty,
            InstrumentId = Guid.Parse(source.InstrumentUid),
            Price = operationTrade.Price.Convert(),
            Quantity = operationTrade.Quantity,
            Amount = new Money(priceAamount, price.Currency),
            TradeId = operationTrade.TradeId,
        };

        return res;
    }

    private static bool IsBySell(Tinkoff.InvestApi.V1.Operation operation)
        => operation.OperationType is
        Tinkoff.InvestApi.V1.OperationType.Sell or
        Tinkoff.InvestApi.V1.OperationType.Buy;

    private static bool IsBySell(Tinkoff.InvestApi.V1.OperationItem operation)
        => operation.Type is
        Tinkoff.InvestApi.V1.OperationType.Sell or
        Tinkoff.InvestApi.V1.OperationType.Buy;
}