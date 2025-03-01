using MediatR;

namespace Vertr.Application.Signals;
internal class ProcessSignalsHandler : IRequestHandler<ProcessSignalsRequest>
{
    public Task Handle(ProcessSignalsRequest request, CancellationToken cancellationToken)
    {
        // Get last signals from db by strategy, symbol, interval
        // For each signal do:
        // - if isHoldAction - skip
        // - get matching accounts
        // - for each account do:
        // - - if is already processed for account - skip
        // - - getAprovedQty
        // - - postOrer
        throw new NotImplementedException();
    }
}
