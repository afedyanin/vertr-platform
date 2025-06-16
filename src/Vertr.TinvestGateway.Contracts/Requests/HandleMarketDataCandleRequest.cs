using MediatR;
using Vertr.TinvestGateway.Contracts.Enums;

namespace Vertr.TinvestGateway.Contracts.Requests;

public class HandleMarketDataCandleRequest : IRequest
{
    public Candle? Candle { get; init; }

    public CandleInterval CandleInterval { get; init; }
}
