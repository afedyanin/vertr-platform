using MediatR;
using Microsoft.Extensions.Logging;
using Vertr.OrderExecution.Application.Abstractions;

namespace Vertr.OrderExecution.Application.Commands;
internal class PostOrderHandler : IRequestHandler<PostOrderRequest, OrderExecutionResponse>
{
    private readonly IOrderExecutionService _orderExecutionService;
    private readonly ILogger<PostOrderHandler> _logger;

    public PostOrderHandler(
        IOrderExecutionService orderExecutionService,
        ILogger<PostOrderHandler> logger)
    {
        _orderExecutionService = orderExecutionService;
        _logger = logger;
    }

    public async Task<OrderExecutionResponse> Handle(PostOrderRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation($"Posting new market order for PortfolioId={request.PortfolioId}");

        var orderId = await _orderExecutionService.PostMarketOrder(
            request.RequestId,
            request.InstrumentId,
            request.QtyLots,
            request.PortfolioId);

        var response = new OrderExecutionResponse
        {
            OrderId = orderId,
        };

        return response;
    }
}
