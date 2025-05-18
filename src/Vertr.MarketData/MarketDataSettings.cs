namespace Vertr.MarketData;

public class MarketDataSettings
{
    public bool IsTinvestMarketDataConsumerEnabled { get; set; }
    public string TinvestMarketDataTopicKey { get; set; } = string.Empty;

    public bool IsCandlesPublisherEnabled { get; set; }

    public string CandlesMarketDataTopicKey { get; set; } = string.Empty;
}
