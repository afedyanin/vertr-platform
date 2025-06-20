using MediatR;
using Microsoft.Extensions.Logging;
using Vertr.PortfolioManager.Application.Abstractions;
using Vertr.PortfolioManager.Application.Entities;
using Vertr.PortfolioManager.Contracts.Requests;

namespace Vertr.PortfolioManager.Application.RequestHandlers;

internal class OrderOperationHandler : IRequestHandler<OrderOperationsRequest>
{
    private readonly IOperationEventRepository _operationEventRepository;
    private readonly ILogger<OrderOperationHandler> _logger;

    public OrderOperationHandler(
        IOperationEventRepository operationEventRepository,
        ILogger<OrderOperationHandler> logger)
    {
        _operationEventRepository = operationEventRepository;
        _logger = logger;
    }

    public async Task Handle(OrderOperationsRequest request, CancellationToken cancellationToken)
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
