using System.Text.Json;
using Vertr.PortfolioManager.Application.Entities;
using Vertr.PortfolioManager.Contracts;

namespace Vertr.PortfolioManager.Application.Converters;
public static class TinvestPortfolioResponseConverter
{
    private static readonly JsonSerializerOptions _jsonSerializerOptions = new JsonSerializerOptions
    {
        WriteIndented = true,
    };

    public static PortfolioSnapshot? Convert(
        this PortfolioResponse? source,
        Guid? bookId)
    {
        if (source == null)
        {
            return null;
        }

        var snapshotId = Guid.NewGuid();

        var res = new PortfolioSnapshot
        {
            Id = snapshotId,
            UpdatedAt = DateTime.UtcNow,
            AccountId = source.AccountId,
            BookId = bookId,
            JsonData = JsonSerializer.Serialize(source, _jsonSerializerOptions),
            JsonDataType = source.GetType().FullName,
        };

        res.Positions = source.Positions.Convert(res);

        return res;
    }

    internal static PortfolioPosition Convert(
        this OrderExecution.Contracts.Position source,
        PortfolioSnapshot parent)
        => new PortfolioPosition
        {
            Id = Guid.NewGuid(),
            PortfolioSnapshotId = parent.Id,
            Balance = source.Balance,
            InstrumentId = new Guid(source.InstrumentId),
            PortfolioSnapshot = parent
        };

    internal static PortfolioPosition[] Convert(
        this OrderExecution.Contracts.Position[] source,
        PortfolioSnapshot parent)
        => source.Select(t => t.Convert(parent)).ToArray();
}
