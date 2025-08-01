using Vertr.MarketData.Contracts.Interfaces;
using Vertr.OrderExecution.Contracts;
using Vertr.OrderExecution.Contracts.Enums;
using Vertr.OrderExecution.Contracts.Interfaces;
using Vertr.Platform.Common.Channels;
using Vertr.PortfolioManager.Contracts;

namespace Vertr.OrderExecution.Application;

internal class OrderExecutionSimulator : IOrderExecutionGateway
{
    private readonly IMarketDataInstrumentRepository _staticMarketDataProvider;
    private readonly IDataProducer<OrderTrades> _orderTradesProducer;

    public OrderExecutionSimulator(
        IMarketDataInstrumentRepository staticMarketDataProvider,
        IDataProducer<OrderTrades> orderTradesProducer)
    {
        _staticMarketDataProvider = staticMarketDataProvider;
        _orderTradesProducer = orderTradesProducer;
    }

    public async Task<PostOrderResponse?> PostOrder(PostOrderRequest request)
    {
        var instrument = await _staticMarketDataProvider.GetById(request.InstrumentId);

        if (instrument == null)
        {
            return null;
        }

        var qty = (long)(request.QuantityLots * (instrument.LotSize ?? 1L));
        var currency = instrument.Currency ?? string.Empty;

        var orderPrice = new Money(request.Price, currency);

        var orderValue = request.Price * qty;
        var orderAmount = new Money(orderValue, currency);

        // TODO: use comission from settings
        var comissionValue = orderValue * 0.03m;
        var comissionAmount = new Money(comissionValue, currency);

        var response = new PostOrderResponse
        {
            OrderId = Guid.NewGuid().ToString(),
            OrderRequestId = request.RequestId.ToString(),
            ExecutionReportStatus = OrderExecutionReportStatus.Fill,
            LotsRequested = request.QuantityLots,
            LotsExecuted = request.QuantityLots,
            InitialOrderPrice = orderPrice,
            ExecutedOrderPrice = orderPrice, // TODO: Use market price
            TotalOrderAmount = orderAmount,
            InitialCommission = comissionAmount,
            ExecutedCommission = comissionAmount,
            InitialSecurityPrice = orderPrice,
            Message = "Simulated order execution",
            InstrumentId = request.InstrumentId,
            OrderType = request.OrderType,
            OrderDirection = request.OrderDirection,
            Source = OrderEventSource.Simulated,
        };

        var trades = new OrderTrades
        {
            InstrumentId = response.InstrumentId,
            CreatedAt = request.CreatedAt,
            Direction = response.OrderDirection,
            OrderId = response.OrderId,
            Trades =
            [
                new Trade
                {
                    TradeId = Guid.NewGuid().ToString(),
                    ExecutionTime = request.CreatedAt,
                    Price = orderPrice,
                    Quantity = qty
                }
            ]
        };

        // Simulating order trades 
        await _orderTradesProducer.Produce(trades);

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
