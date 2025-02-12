using Tinkoff.InvestApi;

namespace Vertr.Adapters.Tinvest;
public class TinvestSettings
{
    public InvestApiSettings? InvestApiSettings { get; set; }

    public string AccountId { get; set; } = string.Empty;

    public IDictionary<string, string> SymbolMappings { get; set; } = new Dictionary<string, string>();
}
