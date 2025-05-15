using MediatR;

namespace Vertr.OrderExecution.Application.Commands;

internal class TradingSignalHandler : IRequestHandler<TradingSignalRequest, TradingSignalResponse>
{
    public Task<TradingSignalResponse> Handle(TradingSignalRequest request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
