using MediatR;
using Vertr.OrderExecution.Contracts;
using Vertr.OrderExecution.Contracts.Enums;
using Vertr.OrderExecution.Contracts.Interfaces;
using Vertr.OrderExecution.Contracts.Requests;

namespace Vertr.OrderExecution.Application;

internal class OrderExecutionSimulator : IOrderExecutionGateway
{
    private readonly IMediator _medator;

    public OrderExecutionSimulator(IMediator mediator)
    {
        _medator = mediator;
    }

    public async Task<PostOrderResponse?> PostOrder(PostOrderRequest request)
    {
        var response = new PostOrderResponse
        {
            OrderId = Guid.NewGuid().ToString(),
            OrderRequestId = request.RequestId.ToString(),
            ExecutionReportStatus = OrderExecutionReportStatus.Fill,
            LotsRequested = request.QuantityLots,
            LotsExecuted = request.QuantityLots,
            InitialOrderPrice = request.Price, // TODO: Use market price
            ExecutedOrderPrice = request.Price,
            TotalOrderAmount = request.Price,
            InitialCommission = request.Price,
            ExecutedCommission = request.Price, // TODO: use comission from settings
            InitialSecurityPrice = request.Price,
            Message = "Simulated order execution",
            InstrumentId = request.InstrumentId,
            OrderType = request.OrderType,
            OrderDirection = request.OrderDirection,
            Source = OrderEventSource.Simulated,
        };

        var tradesRequest = new OrderTradesRequest
        {
            InstrumentId = request.InstrumentId,
            OrderTrades = new OrderTrades
            {
                InstrumentId = response.InstrumentId,
                CreatedAt = DateTime.UtcNow,
                Direction = response.OrderDirection,
                OrderId = response.OrderId,
                Trades = [] // TODO: Implement this
            },
        };

        // Simulate order trades received
        await _medator.Send(tradesRequest);

        return response;
    }

    public Task<DateTime> CancelOrder(string accountId, string orderId)
    {
        throw new NotImplementedException();
    }

    public Task<OrderState?> GetOrderState(string accountId, string orderId, PriceType priceType = PriceType.Unspecified)
    {
        throw new NotImplementedException();
    }

}
