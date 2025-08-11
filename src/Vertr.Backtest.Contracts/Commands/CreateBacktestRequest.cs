using Vertr.Platform.Common.Mediator;

namespace Vertr.Backtest.Contracts.Commands;
public class CreateBacktestRequest : IRequest<CreateBacktestResponse>
{
    public DateTime From { get; init; }

    public DateTime To { get; init; }

    public string? Description { get; init; }

    public Guid StrategyId { get; init; }

    public Guid InsrumentId { get; init; }

    public Guid SubAccountId { get; init; }

    public bool StartImmediately { get; init; }
}
