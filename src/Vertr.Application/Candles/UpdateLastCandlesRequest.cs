using MediatR;
using Vertr.Domain.Enums;

namespace Vertr.Application.Candles;
public class UpdateLastCandlesRequest : IRequest
{
    public IEnumerable<string> Symbols { get; set; } = [];

    public CandleInterval Interval { get; set; }
}
