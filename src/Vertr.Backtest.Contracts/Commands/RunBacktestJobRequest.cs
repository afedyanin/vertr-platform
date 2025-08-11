using Vertr.Platform.Common.Mediator;

namespace Vertr.Backtest.Contracts.Commands;

public class RunBacktestJobRequest : IRequest
{
    public Guid BacktestId { get; init; }
}

