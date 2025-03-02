using MediatR;
using Vertr.Domain.Enums;

namespace Vertr.Application.Candles;
public class UpdateLastCandlesRequest : IRequest
{
    public IEnumerable<(string, CandleInterval)> Symbols { get; set; } = [];
}
