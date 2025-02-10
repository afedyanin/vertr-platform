using Google.Protobuf.Collections;
using Vertr.Domain;

namespace Vertr.Adapters.Tinvest.Converters;

internal static class InstrumentConverter
{
    public static IEnumerable<Instrument> Convert(this RepeatedField<Tinkoff.InvestApi.V1.InstrumentShort> instruments)
    {
        foreach (var instrument in instruments)
        {
            yield return instrument.Convert();
        }
    }

    public static Instrument Convert(this Tinkoff.InvestApi.V1.InstrumentShort instrument)
        => new Instrument
        {
            ClassCode = instrument.ClassCode,
            InstrumentKind = instrument.InstrumentKind.Convert(),
            InstrumentType = instrument.InstrumentType,
            Isin = instrument.Isin,
            Name = instrument.Name,
            Ticker = instrument.Ticker,
            Uid = instrument.Uid,
        };
}
