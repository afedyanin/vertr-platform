using Vertr.MarketData.Contracts;

namespace Vertr.TinvestGateway.Application.Converters;

internal static class InstrumentConverter
{
    public static Instrument ToInstrument(this Tinkoff.InvestApi.V1.InstrumentShort instrument)
        => new Instrument
        {
            InstrumentIdentity = new InstrumentIdentity(
                instrument.ClassCode,
                instrument.Ticker,
                Guid.Parse(instrument.Uid)
                ),
            InstrumentType = instrument.InstrumentType,
            Name = instrument.Name,
            InstrumentKind = instrument.InstrumentKind.ToString(),
        };

public static Instrument ToInstrument(this Tinkoff.InvestApi.V1.Instrument instrument)
        => new Instrument
        {
            InstrumentIdentity = new InstrumentIdentity(
                instrument.ClassCode,
                instrument.Ticker,
                Guid.Parse(instrument.Uid)
                ),
            InstrumentType = instrument.InstrumentType,
            Name = instrument.Name,
            InstrumentKind = instrument.InstrumentKind.ToString(),
            Currency = instrument.Currency,
            LotSize = instrument.Lot
        };

    public static Instrument[] ToInstruments(this Tinkoff.InvestApi.V1.InstrumentShort[] instruments)
        => [.. instruments.Select(ToInstrument)];
}
