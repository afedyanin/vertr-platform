using Vertr.Backtest.Contracts;
using Vertr.PortfolioManager.Contracts;
using Vertr.Strategies.Contracts;

namespace Vertr.Platform.Host.Components.Models;

public class BacktestModel
{
    public required BacktestRun Backtest { get; init; }

    public required StrategyMetadata Strategy { get; set; }

    public required Portfolio Portfolio { get; set; }

    public bool StartImmediately { get; set; }

    public DateTime? SelectedDateFrom { get; set; } = DateTime.UtcNow.AddDays(-10);

    public DateTime? SelectedTimeFrom { get; set; }

    public DateTime? SelectedDateTo { get; set; } = DateTime.UtcNow;

    public DateTime? SelectedTimeTo { get; set; }

    public bool DoStart { get; set; }

    public bool DoCancel { get; set; }

    public DateTime ComposeDateFrom()
    {
        var fromDay = SelectedDateFrom.HasValue ? SelectedDateFrom.Value.Date : DateTime.UtcNow.Date;

        if (SelectedTimeFrom.HasValue)
        {
            return new DateTime(fromDay.Year, fromDay.Month, fromDay.Day, SelectedTimeFrom.Value.Hour, SelectedTimeFrom.Value.Minute, 0, DateTimeKind.Utc);
        }

        return new DateTime(fromDay.Year, fromDay.Month, fromDay.Day, 0, 0, 0, DateTimeKind.Utc);
    }

    public DateTime ComposeDateTo()
    {
        var toDay = SelectedDateTo.HasValue ? SelectedDateTo.Value.Date : DateTime.UtcNow.Date;

        if (SelectedTimeTo.HasValue)
        {
            return new DateTime(toDay.Year, toDay.Month, toDay.Day, SelectedTimeTo.Value.Hour, SelectedTimeTo.Value.Minute, 0, DateTimeKind.Utc);
        }

        return new DateTime(toDay.Year, toDay.Month, toDay.Day, 0, 0, 0, DateTimeKind.Utc);
    }
}
