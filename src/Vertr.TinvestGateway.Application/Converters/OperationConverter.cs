using Vertr.OrderExecution.Contracts;
using Vertr.PortfolioManager.Contracts;

namespace Vertr.TinvestGateway.Application.Converters;

internal static class OperationConverter
{
    public static TradeOperation Convert(this Tinkoff.InvestApi.V1.Operation source)
        => new TradeOperation
        {
            ParentOperationId = source.ParentOperationId,
            Currency = source.Currency,
            Payment = source.Payment,
            Price = source.Price,
            State = source.State.Convert(),
            Quantity = source.Quantity,
            QuantityRest = source.QuantityRest,
            InstrumentType = source.InstrumentType,
            Date = source.Date.ToDateTime(),
            Type = source.Type,
            OperationType = source.OperationType.Convert(),
            AssetUid = source.AssetUid,
            PositionUid = source.PositionUid,
            InstrumentId = source.InstrumentUid,
            OperationTrades = source.Trades.ToArray().Convert()
        };

    public static TradeOperation[] Convert(this Tinkoff.InvestApi.V1.Operation[] source)
        => [.. source.Select(Convert)];

    public static Trade Convert(this Tinkoff.InvestApi.V1.OperationTrade source)
        => new Trade
        {
            ExecutionTime = source.DateTime.ToDateTime(),
            Price = source.Price,
            Quantity = source.Quantity,
            TradeId = source.TradeId,
        };

    public static Trade[] Convert(this Tinkoff.InvestApi.V1.OperationTrade[] source)
        => [.. source.Select(t => t.Convert())];
}
