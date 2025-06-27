using MediatR;
using Microsoft.Extensions.Logging;
using Vertr.PortfolioManager.Application.Extensions;
using Vertr.PortfolioManager.Contracts.Interfaces;
using Vertr.PortfolioManager.Contracts.Requests;

namespace Vertr.PortfolioManager.Application.RequestHandlers;

internal class TradeOperationsHandler : IRequestHandler<TradeOperationsRequest>
{
    private readonly IOperationEventRepository _operationEventRepository;
    private readonly ILogger<TradeOperationsHandler> _logger;

    public TradeOperationsHandler(
        IOperationEventRepository operationEventRepository,
        ILogger<TradeOperationsHandler> logger)
    {
        _operationEventRepository = operationEventRepository;
        _logger = logger;
    }

    public async Task Handle(TradeOperationsRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation($"Order operations received.");

        var operationEvents = request.Operations!.Convert();

        if (operationEvents == null)
        {
            _logger.LogWarning($"Cannot convert operation events.");
            return;
        }

        var saved = await _operationEventRepository.Save(operationEvents);

        if (!saved)
        {
            _logger.LogWarning($"Cannot save operation events.");
        }

        // TODO: Implement operation event handling

    }
}
