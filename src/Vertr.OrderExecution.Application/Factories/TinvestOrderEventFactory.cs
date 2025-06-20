using System.Diagnostics;
using System.Text.Json;
using Vertr.OrderExecution.Application.Entities;
using Vertr.OrderExecution.Contracts;
using Vertr.TinvestGateway.Contracts;

namespace Vertr.OrderExecution.Application.Factories;

internal static class TinvestOrderEventFactory
{
    private static readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
    {
        WriteIndented = true,
    };

    public static OrderEvent CreateEvent(this PostOrderRequest request, PortfolioIdentity portfolioId)
    {
        Debug.Assert(request.AccountId == portfolioId.AccountId);

        return new OrderEvent
        {
            Id = Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow,
            AccountId = portfolioId.AccountId,
            BookId = portfolioId.BookId,
            InstrumentId = request.InstrumentId,
            RequestId = request.RequestId,
            JsonDataType = request.GetType().FullName,
            JsonData = JsonSerializer.Serialize(request, _jsonOptions),
        };
    }

    public static OrderEvent CreateEvent(this PostOrderResponse response, PortfolioIdentity portfolioId)
    {
        return new OrderEvent
        {
            Id = Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow,
            AccountId = portfolioId.AccountId,
            BookId = portfolioId.BookId,
            RequestId = Guid.Parse(response.OrderRequestId),
            OrderId = response.OrderId,
            JsonDataType = response.GetType().FullName,
            JsonData = JsonSerializer.Serialize(response, _jsonOptions),
            InstrumentId = Guid.Parse(response.InstrumentId),
        };

    }

    public static OrderEvent CreateEvent(this OrderState state, PortfolioIdentity portfolioId)
    {
        return new OrderEvent
        {
            Id = Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow,
            AccountId = portfolioId.AccountId,
            BookId = portfolioId.BookId,
            InstrumentId = Guid.Parse(state.InstrumentId),
            RequestId = Guid.Parse(state.OrderRequestId),
            OrderId = state.OrderId,
            JsonDataType = state.GetType().FullName,
            JsonData = JsonSerializer.Serialize(state, _jsonOptions)

        };
    }

    public static OrderEvent CreateEvent(this OrderTrades trades, PortfolioIdentity portfolioId)
    {
        Debug.Assert(trades.AccountId == portfolioId.AccountId);

        return new OrderEvent
        {
            Id = Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow,
            AccountId = portfolioId.AccountId,
            BookId = portfolioId.BookId,
            InstrumentId = Guid.Parse(trades.InstrumentId),
            OrderId = trades.OrderId,
            JsonDataType = trades.GetType().FullName,
            JsonData = JsonSerializer.Serialize(trades, _jsonOptions)
        };
    }
}
