namespace Vertr.MarketData.Contracts;

public record class CandleSubscription
{
    public Guid Id { get; set; }

    public Guid InstrumentId { get; set; }

    public CandleInterval Interval { get; set; }

    public string? ExternalStatus { get; set; }

    public string? ExternalSubscriptionId { get; set; }

    public bool Disabled { get; set; }

    public bool LoadHistory { get; set; }
}
