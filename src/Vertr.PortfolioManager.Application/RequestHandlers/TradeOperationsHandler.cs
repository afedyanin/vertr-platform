using MediatR;
using Microsoft.Extensions.Logging;
using Vertr.PortfolioManager.Contracts;
using Vertr.PortfolioManager.Contracts.Interfaces;
using Vertr.PortfolioManager.Contracts.Requests;

namespace Vertr.PortfolioManager.Application.RequestHandlers;

internal class TradeOperationsHandler : IRequestHandler<TradeOperationsRequest>
{
    private readonly ITradeOperationRepository _operationEventRepository;
    private readonly IPortfolioRepository _portfolioRepository;
    private readonly ITradeOperationService _tadeOperationService;
    private readonly ILogger<TradeOperationsHandler> _logger;

    public TradeOperationsHandler(
        ITradeOperationRepository operationEventRepository,
        IPortfolioRepository portfolioRepository,
        ITradeOperationService tadeOperationService,
        ILogger<TradeOperationsHandler> logger)
    {
        _operationEventRepository = operationEventRepository;
        _portfolioRepository = portfolioRepository;
        _tadeOperationService = tadeOperationService;
        _logger = logger;
    }

    public async Task Handle(TradeOperationsRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation($"Order operations received.");

        await SaveOperations(request.Operations);

        foreach (var operation in request.Operations)
        {
            var portfolio = await _tadeOperationService.ApplyOperation(operation);
            _portfolioRepository.Save(portfolio);
        }
    }

    private async Task SaveOperations(TradeOperation[] operations)
    {
        var saved = await _operationEventRepository.Save(operations);

        if (saved)
        {
            return;
        }

        var ops = string.Join(',', operations.Select(op => op.OrderId));
        var message = $"Cannot save operation events for Orders={ops}";
        throw new InvalidOperationException(message);
    }
}
