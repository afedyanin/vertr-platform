using System.Diagnostics;
using System.Text.Json;
using Vertr.MarketData.Contracts;
using Vertr.OrderExecution.Contracts;

namespace Vertr.OrderExecution.Application.Factories;

internal static class TinvestOrderEventFactory
{
    private static readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
    {
        WriteIndented = true,
    };

    public static OrderEvent CreateEvent(
        this PostOrderRequest request,
        InstrumentIdentity instrumentIdentity,
        PortfolioIdentity portfolioIdentity)
    {
        Debug.Assert(request.AccountId == portfolioIdentity.AccountId);

        return new OrderEvent
        {
            Id = Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow,
            AccountId = portfolioIdentity.AccountId,
            BookId = portfolioIdentity.BookId,
            ClassCode = instrumentIdentity.ClassCode!,
            Ticker = instrumentIdentity.Ticker!,
            RequestId = request.RequestId,
            JsonDataType = request.GetType().FullName,
            JsonData = JsonSerializer.Serialize(request, _jsonOptions),
        };
    }

    public static OrderEvent CreateEvent(
        this PostOrderResponse response,
        InstrumentIdentity instrumentIdentity,
        PortfolioIdentity portfolioIdentity)
    {
        return new OrderEvent
        {
            Id = Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow,
            AccountId = portfolioIdentity.AccountId,
            BookId = portfolioIdentity.BookId,
            ClassCode = instrumentIdentity.ClassCode!,
            Ticker = instrumentIdentity.Ticker!,
            RequestId = Guid.Parse(response.OrderRequestId),
            OrderId = response.OrderId,
            JsonDataType = response.GetType().FullName,
            JsonData = JsonSerializer.Serialize(response, _jsonOptions),
        };

    }

    public static OrderEvent CreateEvent(
        this OrderTrades trades,
        InstrumentIdentity instrumentIdentity,
        PortfolioIdentity portfolioIdentity)
    {
        Debug.Assert(trades.AccountId == portfolioIdentity.AccountId);

        return new OrderEvent
        {
            Id = Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow,
            AccountId = portfolioIdentity.AccountId,
            BookId = portfolioIdentity.BookId,
            ClassCode = instrumentIdentity.ClassCode!,
            Ticker = instrumentIdentity.Ticker!,
            OrderId = trades.OrderId,
            JsonDataType = trades.GetType().FullName,
            JsonData = JsonSerializer.Serialize(trades, _jsonOptions)
        };
    }
}
