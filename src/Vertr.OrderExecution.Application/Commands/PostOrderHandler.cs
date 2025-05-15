using MediatR;
using Vertr.OrderExecution.Application.Abstractions;

namespace Vertr.OrderExecution.Application.Commands;
internal class PostOrderHandler : IRequestHandler<PostOrderRequest, PostOrderResponse>
{
    private readonly IOrderExecutionService _orderExecutionService;

    public PostOrderHandler(IOrderExecutionService orderExecutionService)
    {
        _orderExecutionService = orderExecutionService;
    }

    public async Task<PostOrderResponse> Handle(PostOrderRequest request, CancellationToken cancellationToken)
    {
        var result = await _orderExecutionService.PostMarketOrder(
            request.RequestId,
            request.InstrumentId,
            request.AccountId,
            request.QtyLots);

        var response = new PostOrderResponse
        {
            PostOrderResult = result,
        };

        return response;
    }
}
