using System.Text.Json;
using Vertr.PortfolioManager.Contracts;

namespace Vertr.PortfolioManager.Application.Extensions;

internal static class OperationEventExtensions
{
    private static readonly JsonSerializerOptions _jsonSerializerOptions = new JsonSerializerOptions
    {
        WriteIndented = true,
    };

    public static OperationEvent Convert(this OrderExecution.Contracts.TradeOperation source)
        => new OperationEvent
        {
            AccountId = source.PortfolioIdentity.AccountId,
            BookId = source.PortfolioIdentity.BookId,
            CreatedAt = source.CreatedAt,
            Id = source.Id,
            JsonData = JsonSerializer.Serialize(source, _jsonSerializerOptions),
            JsonDataType = source.GetType().FullName,
        };

    public static OperationEvent[] Convert(this OrderExecution.Contracts.TradeOperation[] source)
        => [.. source.Select(Convert)];
}
