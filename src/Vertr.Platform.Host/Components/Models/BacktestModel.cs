using Vertr.Backtest.Contracts;
using Vertr.MarketData.Contracts;

namespace Vertr.Platform.Host.Components.Models;

public class BacktestModel
{
    public required BacktestRun Backtest { get; init; }

    public required Instrument Instrument { get; set; }
}
