using System.Diagnostics;
using System.Text.Json;
using Vertr.OrderExecution.Contracts;
using Vertr.PortfolioManager.Contracts;

namespace Vertr.OrderExecution.Application.Factories;

internal static class OrderEventFactory
{
    private static readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
    {
        WriteIndented = true,
    };

    public static OrderEvent CreateEvent(
        this PostOrderRequest request,
        Guid instrumentId,
        PortfolioIdentity portfolioIdentity)
    {
        Debug.Assert(request.AccountId == portfolioIdentity.AccountId);

        return new OrderEvent
        {
            Id = Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow,
            AccountId = portfolioIdentity.AccountId,
            SubAccountId = portfolioIdentity.SubAccountId,
            InstrumentId = instrumentId,
            RequestId = request.RequestId,
            JsonDataType = request.GetType().FullName,
            JsonData = JsonSerializer.Serialize(request, _jsonOptions),
        };
    }

    public static OrderEvent CreateEvent(
        this PostOrderResponse response,
        Guid instrumentId,
        PortfolioIdentity portfolioIdentity)
    {
        return new OrderEvent
        {
            Id = Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow,
            AccountId = portfolioIdentity.AccountId,
            SubAccountId = portfolioIdentity.SubAccountId,
            InstrumentId = instrumentId,
            RequestId = Guid.Parse(response.OrderRequestId),
            OrderId = response.OrderId,
            JsonDataType = response.GetType().FullName,
            JsonData = JsonSerializer.Serialize(response, _jsonOptions),
        };

    }

    public static OrderEvent CreateEvent(
        this OrderTrades trades,
        Guid instrumentId,
        PortfolioIdentity portfolioIdentity)
    {
        return new OrderEvent
        {
            Id = Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow,
            AccountId = portfolioIdentity.AccountId,
            SubAccountId = portfolioIdentity.SubAccountId,
            InstrumentId = instrumentId,
            OrderId = trades.OrderId,
            JsonDataType = trades.GetType().FullName,
            JsonData = JsonSerializer.Serialize(trades, _jsonOptions)
        };
    }
}
