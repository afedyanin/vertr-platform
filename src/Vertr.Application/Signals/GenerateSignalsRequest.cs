using MediatR;
using Vertr.Domain;
using Vertr.Domain.Enums;

namespace Vertr.Application.Signals;
public class GenerateSignalsRequest : IRequest
{
    public IEnumerable<string> Symbols { get; set; } = [];

    public CandleInterval Interval { get; set; }

    public PredictorType PredictorType { get; set; } = PredictorType.Undefined;

    public Sb3Algo Sb3Algo { get; set; } = Sb3Algo.Undefined;
}
