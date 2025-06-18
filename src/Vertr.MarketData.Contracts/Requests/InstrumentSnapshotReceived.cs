using MediatR;

namespace Vertr.MarketData.Contracts.Requests;
public class InstrumentSnapshotReceivedRequest : IRequest
{
    public Instrument[]? Instruments { get; init; }
}
