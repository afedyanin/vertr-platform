using Vertr.Backtest.Contracts;
using Vertr.Strategies.Contracts;

namespace Vertr.Platform.Host.Components.Models;

public class BacktestModel
{
    public required BacktestRun Backtest { get; init; }

    public required StrategyMetadata Strategy { get; set; }
}
