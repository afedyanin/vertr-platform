using System.Text.Json;

namespace Vertr.PortfolioManager.Application.Entities;

internal static class OperationEventExtensions
{
    private static readonly JsonSerializerOptions _jsonSerializerOptions = new JsonSerializerOptions
    {
        WriteIndented = true,
    };

    public static OperationEvent Convert(this OrderExecution.Contracts.TradeOperation source)
        => new OperationEvent
        {
            AccountId = source.AccountId,
            BookId = source.BookId,
            CreatedAt = source.CreatedAt,
            Id = source.Id,
            JsonData = JsonSerializer.Serialize(source, _jsonSerializerOptions),
            JsonDataType = source.GetType().FullName,
        };

    public static OperationEvent[] Convert(this OrderExecution.Contracts.TradeOperation[] source)
        => [.. source.Select(Convert)];
}
