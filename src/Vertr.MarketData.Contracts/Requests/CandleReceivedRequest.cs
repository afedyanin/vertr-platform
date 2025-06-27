using MediatR;

namespace Vertr.MarketData.Contracts.Requests;

public class CandleReceivedRequest : IRequest
{
    public required string InstrumentId { get; init; }

    public Candle? Candle { get; init; }
}
