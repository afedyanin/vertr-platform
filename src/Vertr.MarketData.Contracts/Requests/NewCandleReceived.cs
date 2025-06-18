using MediatR;

namespace Vertr.MarketData.Contracts.Requests;

public class NewCandleReceivedRequest : IRequest
{
    public Candle? Candle { get; init; }

    public string? InstrumentId { get; init; }

    public CandleInterval? Interval { get; init; }
}
