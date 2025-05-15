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
        };

        res.Positions = source.Positions.Convert(res);

        return res;
    }

    public static PortfolioPosition Convert(
        this TinvestGateway.Contracts.Position source,
        PortfolioSnapshot parent)
        => new PortfolioPosition
        {
            Id = Guid.NewGuid(),
            PortfolioSnapshotId = parent.Id,
            Balance = source.Balance,
            InstrumentId = new Guid(source.InstrumentId),
            PortfolioSnapshot = parent
        };

    public static PortfolioPosition[] Convert(
        this TinvestGateway.Contracts.Position[] source,
        PortfolioSnapshot parent)
        => source.Select(t => t.Convert(parent)).ToArray();
}
