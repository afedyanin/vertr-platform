using MediatR;
using Vertr.Domain.Settings;

namespace Vertr.Application.Signals;
public class GenerateSignalsRequest : IRequest
{
    public IEnumerable<StrategySettings> Strategies { get; set; } = [];
}
