using System.Text.Json;
using Vertr.PortfolioManager.Application.Entities;

namespace Vertr.PortfolioManager.Converters;

internal static class OperationEventConverter
{
    private static readonly JsonSerializerOptions _jsonSerializerOptions = new JsonSerializerOptions
    {
        WriteIndented = true,
    };

    public static OperationEvent Convert(this OrderExecution.Contracts.OrderOperation source)
        => new OperationEvent
        {
            AccountId = source.AccountId,
            BookId = source.BookId,
            CreatedAt = source.CreatedAt,
            Id = source.Id,
            JsonData = JsonSerializer.Serialize(source, _jsonSerializerOptions),
            JsonDataType = source.GetType().FullName,
        };
}
