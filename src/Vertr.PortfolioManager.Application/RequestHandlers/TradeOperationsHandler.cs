using MediatR;
using Microsoft.Extensions.Logging;
using Vertr.PortfolioManager.Application.Extensions;
using Vertr.PortfolioManager.Contracts;
using Vertr.PortfolioManager.Contracts.Interfaces;
using Vertr.PortfolioManager.Contracts.Requests;

namespace Vertr.PortfolioManager.Application.RequestHandlers;

internal class TradeOperationsHandler : IRequestHandler<TradeOperationsRequest>
{
    private readonly ITradeOperationRepository _operationEventRepository;
    private readonly IPortfolioRepository _portfolioRepository;
    private readonly ILogger<TradeOperationsHandler> _logger;

    public TradeOperationsHandler(
        ITradeOperationRepository operationEventRepository,
        IPortfolioRepository portfolioRepository,
        ILogger<TradeOperationsHandler> logger)
    {
        _operationEventRepository = operationEventRepository;
        _portfolioRepository = portfolioRepository;
        _logger = logger;
    }

    public async Task Handle(TradeOperationsRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation($"Order operations received.");

        var saved = await _operationEventRepository.Save(request.Operations);

        if (!saved)
        {
            _logger.LogWarning($"Cannot save operation events.");
        }

        foreach (var operation in request.Operations)
        {
            var portfolioIdentity = new PortfolioIdentity(operation.AccountId, operation.BookId);
            var portfolio = _portfolioRepository.GetPortfolio(portfolioIdentity);

            portfolio ??= new Portfolio
            {
                Identity = portfolioIdentity,
                UpdatedAt = DateTime.UtcNow,
            };

            var updated = await portfolio.ApplyOperation(operation);
            _portfolioRepository.Save(updated);
        }
    }
}
