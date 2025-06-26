using System.Text.Json.Serialization;

namespace Vertr.MarketData.Contracts;

public record class InstrumentIdentity
{
    public Guid? Id { get; init; }

    public string? ClassCode { get; init; }

    public string? Ticker { get; init; }

    [JsonConstructor]
    private InstrumentIdentity() { }

    public InstrumentIdentity(Guid id)
    {
        Id = id;
    }

    public InstrumentIdentity(
        string classCode,
        string ticker,
        Guid? id = null)
    {
        ClassCode = classCode;
        Ticker = ticker;
        Id = id;
    }
}
