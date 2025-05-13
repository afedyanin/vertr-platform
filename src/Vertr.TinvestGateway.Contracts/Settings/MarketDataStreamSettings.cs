namespace Vertr.TinvestGateway.Contracts.Settings;

public class MarketDataStreamSettings
{
    public bool WaitCandleClose { get; set; }

    public bool IsEnabled { get; set; }

    public string TopicKey { get; set; } = "MarketDataStream.Candles";

    public IDictionary<string, CandleInterval> Candles { get; set; } = new Dictionary<string, CandleInterval>();

    public CandleInterval GetInterval(string symbol)
        => Candles.TryGetValue(symbol, out var interval) ? interval : CandleInterval.Unspecified;

}


