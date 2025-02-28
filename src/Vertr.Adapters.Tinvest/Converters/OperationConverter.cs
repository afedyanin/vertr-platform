using AutoMapper;

namespace Vertr.Adapters.Tinvest.Converters;
public class OperationConverter : ITypeConverter<Tinkoff.InvestApi.V1.Operation, Domain.Operation>

{
    public Domain.Operation Convert(
        Tinkoff.InvestApi.V1.Operation source,
        Domain.Operation destination,
        ResolutionContext context)
    {
        destination = new Domain.Operation
        {
            Id = ToGuid(source.Id)!.Value,
            ParentOperationId = ToGuid(source.ParentOperationId),
            AssetUid = ToGuid(source.AssetUid),
            Date = source.Date.ToDateTime(),
            Currency = source.Currency,
            InstrumentType = source.InstrumentType,
            InstrumentUid = ToGuid(source.InstrumentUid)!.Value,
            PositionUid = ToGuid(source.PositionUid)!.Value,
            OperationType = context.Mapper.Map<Domain.Enums.OperationType>(source.OperationType),
            Payment = source.Payment,
            Price = source.Price,
            Quantity = source.Quantity,
            QuantityRest = source.QuantityRest,
            State = context.Mapper.Map<Domain.Enums.OperationState>(source.State),
            Type = source.Type,
            AccountId = string.Empty,
            OperationTrades = context.Mapper.Map<Domain.OperationTrade[]>(source.Trades.ToArray())
        };

        return destination;
    }

    private static Guid? ToGuid(string? value)
        => string.IsNullOrEmpty(value) ? null : Guid.Parse(value);
}
