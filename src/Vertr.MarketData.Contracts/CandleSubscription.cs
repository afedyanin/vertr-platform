namespace Vertr.MarketData.Contracts;

public class CandleSubscription
{
    public Guid Id { get; set; }

    public required string Symbol { get; set; }

    public CandleInterval CandleInterval { get; set; }

    public bool WaitingClose { get; set; }

    public string? ExternalId { get; set; }

    public string? ExternalStatus { get; set; }
}
