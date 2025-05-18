using MediatR;
using Vertr.MarketData.Contracts;

namespace Vertr.MarketData.Application.Commands;

public class NewCandleRequest : IRequest
{
    public required Candle Candle { get; init; }
}
