using Vertr.Domain;

namespace Vertr.Adapters.Tinvest.Converters;
internal static class InstrumentTypeConverter
{
    public static InstrumentType Convert(this Tinkoff.InvestApi.V1.InstrumentType instrumentIdType)
    {
        return instrumentIdType switch
        {
            Tinkoff.InvestApi.V1.InstrumentType.Unspecified => InstrumentType.Unspecified,
            Tinkoff.InvestApi.V1.InstrumentType.Bond => InstrumentType.Bond,
            Tinkoff.InvestApi.V1.InstrumentType.Share => InstrumentType.Share,
            Tinkoff.InvestApi.V1.InstrumentType.Currency => InstrumentType.Currency,
            Tinkoff.InvestApi.V1.InstrumentType.Etf => InstrumentType.Etf,
            Tinkoff.InvestApi.V1.InstrumentType.Futures => InstrumentType.Futures,
            Tinkoff.InvestApi.V1.InstrumentType.Sp => InstrumentType.Sp,
            Tinkoff.InvestApi.V1.InstrumentType.Option => InstrumentType.Option,
            Tinkoff.InvestApi.V1.InstrumentType.ClearingCertificate => InstrumentType.ClearingCertificate,
            Tinkoff.InvestApi.V1.InstrumentType.Index => InstrumentType.Index,
            Tinkoff.InvestApi.V1.InstrumentType.Commodity => InstrumentType.Commodity,
            _ => throw new InvalidOperationException($"Unknown instrument type={instrumentIdType}"),
        };
    }
}
