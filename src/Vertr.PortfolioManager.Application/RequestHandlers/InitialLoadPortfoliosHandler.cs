using MediatR;
using Vertr.PortfolioManager.Contracts.Interfaces;
using Vertr.PortfolioManager.Contracts.Requests;

namespace Vertr.PortfolioManager.Application.RequestHandlers;
internal class InitialLoadPortfoliosHandler : IRequestHandler<InitialLoadPortfoliosRequest>
{
    private readonly ITradeOperationRepository _tradeOperationRepository;
    private readonly IMediator _mediator;

    public InitialLoadPortfoliosHandler(
        ITradeOperationRepository tradeOperationRepository,
        IMediator mediator)
    {
        _tradeOperationRepository = tradeOperationRepository;
        _mediator = mediator;
    }

    public async Task Handle(InitialLoadPortfoliosRequest request, CancellationToken cancellationToken)
    {
        var allOperations = await _tradeOperationRepository.GetAll();

        var opRequest = new TradeOperationsRequest
        {
            Operations = allOperations
        };

        await _mediator.Send(opRequest);
    }
}
