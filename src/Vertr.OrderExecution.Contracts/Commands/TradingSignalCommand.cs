using Vertr.Platform.Common;

namespace Vertr.OrderExecution.Contracts.Commands;

public class TradingSignalCommand : OrderCommandBase, ICommand<ExecuteOrderResponse>
{
    public long QtyLots { get; init; }
}
