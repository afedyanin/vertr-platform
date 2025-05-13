using Tinkoff.InvestApi;

namespace Vertr.TinvestGateway;
public class TinvestSettings
{
    public InvestApiSettings? InvestApiSettings { get; set; }

    public string[] Accounts { get; set; } = [];

    public IDictionary<string, string> SymbolMappings { get; set; } = new Dictionary<string, string>();

    public string? GetSymbolId(string symbol)
        => SymbolMappings.TryGetValue(symbol, out var symbolId) ? symbolId : null;

    public string GetSymbolById(string symbolId)
        => SymbolMappings.FirstOrDefault(kvp => kvp.Value.Equals(symbolId, StringComparison.OrdinalIgnoreCase)).Key;
}
