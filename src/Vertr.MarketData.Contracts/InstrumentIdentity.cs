namespace Vertr.MarketData.Contracts;

public record class InstrumentIdentity
{
    public Guid? Id { get; init; }

    public string? ClassCode { get; init; }
    public string? Ticker { get; init; }

    public string? Isin { get; init; }

    public InstrumentIdentity(Guid id)
    {
        Id = id;
    }

    public InstrumentIdentity(
        string classCode,
        string ticker,
        Guid? id = null,
        string? isin = null)
    {
        ClassCode = classCode;
        Ticker = ticker;
        Id = id;
        Isin = isin;
    }
}
