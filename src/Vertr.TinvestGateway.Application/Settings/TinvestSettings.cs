using Tinkoff.InvestApi;
using Vertr.TinvestGateway.Contracts.Enums;

namespace Vertr.TinvestGateway.Application.Settings;
public class TinvestSettings
{
    public InvestApiSettings? InvestApiSettings { get; set; }

    public bool PositionStreamEnabled { get; set; }

    public bool PortfolioStreamEnabled { get; set; }

    public bool OrderTradeStreamEnabled { get; set; }

    public bool OrderStateStreamEnabled { get; set; }

    public bool MarketDataStreamEnabled { get; set; }

    public bool WaitCandleClose { get; set; }

    public string[] Accounts { get; set; } = [];

    public IDictionary<string, string> SymbolMappings { get; set; } = new Dictionary<string, string>();

    public IDictionary<string, CandleInterval> Candles { get; set; } = new Dictionary<string, CandleInterval>();

    public CandleInterval GetInterval(string symbol)
        => Candles.TryGetValue(symbol, out var interval) ? interval : CandleInterval.Unspecified;

    public string? GetSymbolId(string symbol)
        => SymbolMappings.TryGetValue(symbol, out var symbolId) ? symbolId : null;

    public string GetSymbolById(string symbolId)
        => SymbolMappings.FirstOrDefault(kvp => kvp.Value.Equals(symbolId, StringComparison.OrdinalIgnoreCase)).Key;
}
