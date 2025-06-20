using MediatR;

namespace Vertr.OrderExecution.Contracts.Requests;

public class TradingSignalRequest : IRequest<ExecuteOrderResponse>
{
    public Guid RequestId { get; init; }

    public Guid InstrumentId { get; init; }

    public long QtyLots { get; init; }

    public required PortfolioIdentity PortfolioId { get; init; }
}
