using MediatR;

namespace Vertr.OrderExecution.Contracts.Requests;

public class TradingSignalRequest : OrderRequestBase, IRequest<ExecuteOrderResponse>
{
    public long QtyLots { get; init; }
}
