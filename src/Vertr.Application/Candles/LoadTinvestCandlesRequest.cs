using MediatR;
using Vertr.Domain;

namespace Vertr.Application.Candles;
public class LoadTinvestCandlesRequest : IRequest
{
    public IEnumerable<string> Symbols { get; set; } = [];

    public CandleInterval Interval { get; set; }
}
