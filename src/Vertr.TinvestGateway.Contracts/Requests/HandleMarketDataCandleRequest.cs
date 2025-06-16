using MediatR;

namespace Vertr.TinvestGateway.Contracts.Requests;

public class HandleMarketDataCandleRequest : IRequest
{
    public Candle? Candle { get; init; }
}
