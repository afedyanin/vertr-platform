using MediatR;
using Microsoft.Extensions.Logging;
using Vertr.OrderExecution.Application.Abstractions;

namespace Vertr.OrderExecution.Application.Commands;
internal class PostOrderHandler : IRequestHandler<PostOrderRequest, OrderExecutionResponse>
{
    private readonly IOrderExecutionService _orderExecutionService;
    private readonly IOrderEventRepository _orderEventRepository;
    private readonly ILogger<PostOrderHandler> _logger;

    public PostOrderHandler(
        IOrderExecutionService orderExecutionService,
        IOrderEventRepository orderEventRepository,
        ILogger<PostOrderHandler> logger)
    {
        _orderExecutionService = orderExecutionService;
        _orderEventRepository = orderEventRepository;
        _logger = logger;
    }

    public async Task<OrderExecutionResponse> Handle(PostOrderRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation($"Posting new market order for AccountId={request.AccountId}");

        var result = await _orderExecutionService.PostMarketOrder(
            request.RequestId,
            request.InstrumentId,
            request.AccountId,
            request.QtyLots);

        var response = new OrderExecutionResponse
        {
            OrderId = result.OrderId,
        };

        var saved = await _orderEventRepository.Save(result);

        if (!saved)
        {
            _logger.LogError($"Cannot save post order result for AccountId={request.AccountId} RequestId={request.RequestId}");
        }

        return response;
    }
}
