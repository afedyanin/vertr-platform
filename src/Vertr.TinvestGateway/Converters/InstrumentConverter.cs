using Vertr.MarketData.Contracts;

namespace Vertr.TinvestGateway.Converters;

internal static class InstrumentConverter
{
    public static Instrument ToInstrument(this Tinkoff.InvestApi.V1.InstrumentShort instrument)
        => new Instrument
        {
            Id = Guid.Parse(instrument.Uid),
            Symbol = new Symbol(instrument.ClassCode, instrument.Ticker),
            InstrumentType = instrument.InstrumentType,
            Name = instrument.Name,
        };

public static Instrument ToInstrument(this Tinkoff.InvestApi.V1.Instrument instrument)
        => new Instrument
        {
            Id = Guid.Parse(instrument.Uid),
            Symbol = new Symbol(instrument.ClassCode, instrument.Ticker),
            InstrumentType = instrument.InstrumentType,
            Name = instrument.Name,
            Currency = instrument.Currency,
            LotSize = instrument.Lot
        };

    public static Instrument[] ToInstruments(this Tinkoff.InvestApi.V1.InstrumentShort[] instruments)
        => [.. instruments.Select(ToInstrument)];
}
