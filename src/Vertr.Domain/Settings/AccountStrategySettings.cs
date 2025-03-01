namespace Vertr.Domain.Settings;
public class AccountStrategySettings
{
    public IDictionary<string, IList<StrategySettings>> SignalMappings { get; set; } =
        new Dictionary<string, IList<StrategySettings>>();
}
