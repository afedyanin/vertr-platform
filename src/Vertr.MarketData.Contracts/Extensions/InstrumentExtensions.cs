namespace Vertr.MarketData.Contracts.Extensions;

public static class InstrumentExtensions
{
    public static bool IsCurrency(this Instrument instrument)
        => instrument.InstrumentType is not null &&
           instrument.InstrumentType.Equals("currency", StringComparison.OrdinalIgnoreCase);

    public static IEnumerable<Instrument> FliterOutCurrency(
        this IEnumerable<Instrument>? instruments)
        => instruments?.Where(i => !i.IsCurrency()) ?? [];
}
