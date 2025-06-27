using MediatR;
using Microsoft.Extensions.Logging;
using Vertr.PortfolioManager.Contracts.Interfaces;
using Vertr.PortfolioManager.Contracts.Requests;

namespace Vertr.PortfolioManager.Application.RequestHandlers;

internal class TradeOperationsHandler : IRequestHandler<TradeOperationsRequest>
{
    private readonly ITradeOperationRepository _operationEventRepository;
    private readonly ILogger<TradeOperationsHandler> _logger;

    public TradeOperationsHandler(
        ITradeOperationRepository operationEventRepository,
        ILogger<TradeOperationsHandler> logger)
    {
        _operationEventRepository = operationEventRepository;
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

        // TODO: Implement operation event handling
    }
}
