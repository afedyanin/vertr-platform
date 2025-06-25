using Vertr.MarketData.Contracts;

namespace Vertr.TinvestGateway.Application.Converters;

internal static class InstrumentConverter
{
    public static Instrument ToInstrument(this Tinkoff.InvestApi.V1.InstrumentShort instrument)
        => new Instrument(
            new InstrumentIdentity(
                instrument.ClassCode,
                instrument.Ticker,
                Guid.Parse(instrument.Uid),
                instrument.Isin),
            instrument.InstrumentType,
            instrument.Name,
            instrument.InstrumentKind.ToString(),
            null,
            null
            );

    public static Instrument ToInstrument(this Tinkoff.InvestApi.V1.Instrument instrument)
        => new Instrument(
            new InstrumentIdentity(
                instrument.ClassCode,
                instrument.Ticker,
                Guid.Parse(instrument.Uid),
                instrument.Isin),
            instrument.InstrumentType,
            instrument.Name,
            instrument.InstrumentKind.ToString(),
            instrument.Currency,
            instrument.Lot
            );

    public static Instrument[] ToInstruments(this Tinkoff.InvestApi.V1.InstrumentShort[] instruments)
        => [.. instruments.Select(ToInstrument)];
}
