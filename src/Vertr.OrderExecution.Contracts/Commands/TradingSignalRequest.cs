using Vertr.Platform.Common.Mediator;

namespace Vertr.OrderExecution.Contracts.Commands;

public class TradingSignalRequest : OrderCommandBase, IRequest<ExecuteOrderResponse>
{
    public long QtyLots { get; init; }
}
