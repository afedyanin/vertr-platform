using MediatR;

namespace Vertr.MarketData.Contracts.Requests;
public class InstrumentSnapshotReceived : IRequest
{
    public Instrument[]? Instruments { get; init; }
}
