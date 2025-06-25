using MediatR;
using Vertr.MarketData.Contracts;

namespace Vertr.OrderExecution.Contracts.Requests;

public class OrderTradesRequest : IRequest
{
    public required InstrumentIdentity InstrumentIdentity { get; init; }

    public required OrderTrades OrderTrades { get; init; }
}
