using Vertr.MarketData.Contracts;

namespace Vertr.TinvestGateway.Application.Converters;

internal static class InstrumentConverter
{
    public static Instrument ToInstrument(this Tinkoff.InvestApi.V1.InstrumentShort instrument)
        => new Instrument(
            instrument.Isin,
            instrument.Ticker,
            instrument.ClassCode,
            instrument.InstrumentType,
            instrument.Name,
            instrument.Uid,
            instrument.InstrumentKind.ToString(),
            null,
            null
            );

    public static Instrument ToInstrument(this Tinkoff.InvestApi.V1.Instrument instrument)
        => new Instrument(
            instrument.Isin,
            instrument.Ticker,
            instrument.ClassCode,
            instrument.InstrumentType,
            instrument.Name,
            instrument.Uid,
            instrument.InstrumentKind.ToString(),
            instrument.Currency,
            instrument.Lot
            );

    public static Instrument[] ToInstruments(this Tinkoff.InvestApi.V1.InstrumentShort[] instruments)
        => [.. instruments.Select(ToInstrument)];
}
