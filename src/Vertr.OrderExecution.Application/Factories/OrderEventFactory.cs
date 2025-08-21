using System.Diagnostics;
using System.Text.Json;
using Vertr.OrderExecution.Contracts;
using Vertr.Platform.Common.Utils;

namespace Vertr.OrderExecution.Application.Factories;

internal static class OrderEventFactory
{
    public static OrderEvent CreateEventFromOrderRequest(
        PostOrderRequest request,
        Guid instrumentId,
        Guid portfolioId,
        string accountId)
    {
        Debug.Assert(request.AccountId == accountId);

        return new OrderEvent
        {
            Id = Guid.NewGuid(),
            CreatedAt = request.CreatedAt,
            AccountId = accountId,
            PortfolioId = portfolioId,
            InstrumentId = instrumentId,
            RequestId = request.RequestId,
            JsonDataType = request.GetType().FullName,
            JsonData = JsonSerializer.Serialize(request, JsonOptions.DefaultOptions),
        };
    }

    public static OrderEvent CreateEventFromOrderResponse(
        PostOrderResponse response,
        Guid instrumentId,
        Guid portfolioId,
        string accountId,
        DateTime createdAt)
    {
        return new OrderEvent
        {
            Id = Guid.NewGuid(),
            CreatedAt = createdAt,
            AccountId = accountId,
            PortfolioId = portfolioId,
            InstrumentId = instrumentId,
            RequestId = Guid.Parse(response.OrderRequestId),
            OrderId = response.OrderId,
            JsonDataType = response.GetType().FullName,
            JsonData = JsonSerializer.Serialize(response, JsonOptions.DefaultOptions),
        };
    }

    public static OrderEvent CreateEventFromOrderTrades(
        OrderTrades trades,
        Guid instrumentId,
        Guid portfolioId,
        string accountId)
    {
        return new OrderEvent
        {
            Id = Guid.NewGuid(),
            CreatedAt = trades.CreatedAt,
            AccountId = accountId,
            PortfolioId = portfolioId,
            InstrumentId = instrumentId,
            OrderId = trades.OrderId,
            JsonDataType = trades.GetType().FullName,
            JsonData = JsonSerializer.Serialize(trades, JsonOptions.DefaultOptions)
        };
    }

    public static OrderEvent CreateEventFromOrderState(
        OrderState state,
        Guid portfolioId)
    {
        return new OrderEvent
        {
            Id = Guid.NewGuid(),
            CreatedAt = state.CreatedAt,
            AccountId = state.AccountId ?? string.Empty,
            PortfolioId = portfolioId,
            InstrumentId = state.InstrumentId,
            RequestId = Guid.Parse(state.OrderRequestId),
            OrderId = state.OrderId,
            JsonDataType = state.GetType().FullName,
            JsonData = JsonSerializer.Serialize(state, JsonOptions.DefaultOptions)
        };
    }
}
