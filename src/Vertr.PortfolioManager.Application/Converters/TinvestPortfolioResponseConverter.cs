using System.Text.Json;
using Vertr.PortfolioManager.Application.Entities;

namespace Vertr.PortfolioManager.Application.Converters;
internal static class TinvestPortfolioResponseConverter
{
    private static readonly JsonSerializerOptions _jsonSerializerOptions = new JsonSerializerOptions
    {
        WriteIndented = true,
    };

    public static PortfolioSnapshot? Convert(
        this TinvestGateway.Contracts.PortfolioResponse? source)
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
            JsonData = JsonSerializer.Serialize(source, _jsonSerializerOptions),
            JsonDataType = source.GetType().FullName,
            Positions = source.Positions.Convert(snapshotId)
        };

        return res;
    }

    public static PortfolioPosition Convert(
        this TinvestGateway.Contracts.Position source,
        Guid portfolioSnapshotId)
        => new PortfolioPosition
        {
            Id = Guid.NewGuid(),
            PortfolioSnapshotId = portfolioSnapshotId,
            Balance = source.Balance,
            InstrumentId = new Guid(source.InstrumentId),
        };

    public static PortfolioPosition[] Convert(
        this TinvestGateway.Contracts.Position[] source,
        Guid portfolioSnapshotId)
        => source.Select(t => t.Convert(portfolioSnapshotId)).ToArray();
}
