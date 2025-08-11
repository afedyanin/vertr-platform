using MediatR;

namespace Vertr.OrderExecution.Contracts.Commands;

public class TradingSignalCommand : OrderCommandBase, IRequest<ExecuteOrderResponse>
{
    public long QtyLots { get; init; }
}
