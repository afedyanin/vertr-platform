using System.Text.Json;
using Vertr.OrderExecution.Application.Entities;
using Vertr.TinvestGateway.Contracts;

namespace Vertr.OrderExecution.Application.Factories;

internal static class TinvestOrderEventFactory
{

    public static OrderEvent CreateEvent(this PostOrderRequest request)
        => new OrderEvent
        {
            Id = Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow,
            AccountId = request.AccountId,
            InstrumentId = request.InstrumentId,
            RequestId = request.RequestId,
            JsonDataType = request.GetType().FullName,
            JsonData = JsonSerializer.Serialize(request)
        };

    public static OrderEvent CreateEvent(this PostOrderResponse response)
        => new OrderEvent
        {
            Id = Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow,
            RequestId = Guid.Parse(response.OrderRequestId),
            OrderId = response.OrderId,
            JsonDataType = response.GetType().FullName,
            JsonData = JsonSerializer.Serialize(response)
        };

}
