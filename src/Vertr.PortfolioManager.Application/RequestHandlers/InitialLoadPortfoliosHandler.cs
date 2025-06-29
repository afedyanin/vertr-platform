using MediatR;
using Vertr.PortfolioManager.Contracts.Interfaces;
using Vertr.PortfolioManager.Contracts.Requests;

namespace Vertr.PortfolioManager.Application.RequestHandlers;
internal class InitialLoadPortfoliosHandler : IRequestHandler<InitialLoadPortfoliosRequest>
{
    private readonly ITradeOperationRepository _tradeOperationRepository;
    private readonly ITradeOperationService _tradeOperationService;
    private readonly IPortfolioRepository _portfolioRepository;

    public InitialLoadPortfoliosHandler(
        ITradeOperationRepository tradeOperationRepository,
        ITradeOperationService tradeOperationService,
        IPortfolioRepository portfolioRepository)
    {
        _tradeOperationRepository = tradeOperationRepository;
        _tradeOperationService = tradeOperationService;
        _portfolioRepository = portfolioRepository;
    }

    public async Task Handle(InitialLoadPortfoliosRequest request, CancellationToken cancellationToken)
    {
        var allOperations = await _tradeOperationRepository.GetAll();

        foreach (var operation in allOperations)
        {
            var portfolio = await _tradeOperationService.ApplyOperation(operation);
            _portfolioRepository.Save(portfolio);
        }
    }
}
