using System.Text.Json;
using Vertr.OrderExecution.Application.Entities;
using Vertr.TinvestGateway.Contracts;

namespace Vertr.OrderExecution.Application.Factories;

public static class TinvestOrderEventFactory
{
    private static readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
    {
        WriteIndented = true,
    };

    public static OrderEvent CreateEvent(this PostOrderRequest request, Guid bookId)
        => new OrderEvent
        {
            Id = Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow,
            AccountId = request.AccountId,
            InstrumentId = request.InstrumentId,
            RequestId = request.RequestId,
            JsonDataType = request.GetType().FullName,
            JsonData = JsonSerializer.Serialize(request, _jsonOptions),
            BookId = bookId,
        };

    public static OrderEvent CreateEvent(this PostOrderResponse response, Guid bookId)
        => new OrderEvent
        {
            Id = Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow,
            RequestId = Guid.Parse(response.OrderRequestId),
            OrderId = response.OrderId,
            JsonDataType = response.GetType().FullName,
            JsonData = JsonSerializer.Serialize(response, _jsonOptions),
            InstrumentId = Guid.Parse(response.InstrumentId),
            BookId = bookId,
        };

    public static OrderEvent CreateEvent(this OrderState state)
        => new OrderEvent
        {
            Id = Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow,
            InstrumentId = Guid.Parse(state.InstrumentId),
            RequestId = Guid.Parse(state.OrderRequestId),
            OrderId = state.OrderId,
            JsonDataType = state.GetType().FullName,
            JsonData = JsonSerializer.Serialize(state, _jsonOptions)
        };

    public static OrderEvent CreateEvent(this OrderTrades trades)
        => new OrderEvent
        {
            Id = Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow,
            InstrumentId = Guid.Parse(trades.InstrumentId),
            AccountId = trades.AccountId,
            OrderId = trades.OrderId,
            JsonDataType = trades.GetType().FullName,
            JsonData = JsonSerializer.Serialize(trades, _jsonOptions)
        };

}
