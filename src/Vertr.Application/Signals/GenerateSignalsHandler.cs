using MediatR;

namespace Vertr.Application.Signals;

internal class GenerateSignalsHandler : IRequestHandler<GenerateSignalsRequest>
{
    public Task Handle(GenerateSignalsRequest request, CancellationToken cancellationToken)
    {
        // Get last signal from repo
        // Get last candle from repo
        // If is it new candle - call predictor API
        // Crate new signal from prediction and save it into DB
        throw new NotImplementedException();
    }
}
