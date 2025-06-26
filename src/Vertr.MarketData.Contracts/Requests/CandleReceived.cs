using MediatR;

namespace Vertr.MarketData.Contracts.Requests;

public class CandleReceived : IRequest
{
    public required string InstrumentId { get; init; }

    public Candle? Candle { get; init; }
}
