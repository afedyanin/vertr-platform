namespace Vertr.Domain;
public record class Instrument
{
    public string? Isin { get; init; }

    public string? Ticker { get; init; }

    public string? ClassCode { get; init; }

    public string? InstrumentType { get; init; }

    public string? Name { get; init; }

    public string? Uid { get; init; }

    public InstrumentType InstrumentKind { get; init; }
}

public record class InstrumentDetails : Instrument
{
    public int Lot { get; init; }

    public string? Currency { get; init; }

    public string? Exchange { get; init; }

    public decimal MinPriceIncrement { get; init; }

}
