using System.Text.Json.Serialization;

namespace Vertr.MarketData.Contracts;

public record class InstrumentIdentity
{
    public Guid Id { get; init; } = Guid.Empty;

    public string ClassCode { get; init; } = string.Empty;

    public string Ticker { get; init; } = string.Empty;

    public string Symbol => $"{ClassCode}.{Ticker}";

    public bool HasUid => Id != Guid.Empty;

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
        Id = id ?? Guid.Empty;
    }
}
