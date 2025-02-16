using MediatR;
using Vertr.Domain;

namespace Vertr.Application.Candles;
public class GenerateSignalsRequest : IRequest
{
    public IEnumerable<string> Symbols { get; set; } = [];

    public CandleInterval Interval { get; set; }
}
