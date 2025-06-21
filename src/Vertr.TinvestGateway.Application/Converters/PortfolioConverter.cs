using System.Text.Json;
using Vertr.PortfolioManager.Contracts;
using Vertr.TinvestGateway.Contracts;

namespace Vertr.TinvestGateway.Application.Converters;

internal static class PortfolioConverter
{
    private static readonly JsonSerializerOptions _jsonSerializerOptions = new JsonSerializerOptions
    {
        WriteIndented = true,
    };

    public static PortfolioSnapshot? Convert(
        this Tinkoff.InvestApi.V1.PortfolioResponse source,
        Guid? bookId)
    {
        if (source == null)
        {
            return null;
        }

        var response = source.Convert();

        var res = new PortfolioSnapshot
        {
            Id = Guid.NewGuid(),
            UpdatedAt = DateTime.UtcNow,
            AccountId = response.AccountId,
            BookId = bookId,
            JsonData = JsonSerializer.Serialize(response, _jsonSerializerOptions),
            JsonDataType = response.GetType().FullName,
        };

        res.Positions = response.Positions.Convert(res.Id);

        return res;
    }

    private static PortfolioResponse Convert(this Tinkoff.InvestApi.V1.PortfolioResponse source)
        => new PortfolioResponse
        {
            AccountId = source.AccountId,
            TotalAmountShares = source.TotalAmountShares,
            TotalAmountBonds = source.TotalAmountBonds,
            TotalAmountEtf = source.TotalAmountEtf,
            TotalAmountCurrencies = source.TotalAmountCurrencies,
            TotalAmountFutures = source.TotalAmountFutures,
            TotalAmountOptions = source.TotalAmountOptions,
            TotalAmountSp = source.TotalAmountSp,
            TotalAmountPortfolio = source.TotalAmountPortfolio,
            ExpectedYield = source.ExpectedYield,
            Positions = source.Positions.ToArray().Convert(),
        };

    private static PortfolioPosition Convert(
        this Position source,
        Guid parentSnapshotId)
        => new PortfolioPosition
        {
            Id = Guid.NewGuid(),
            PortfolioSnapshotId = parentSnapshotId,
            Balance = source.Balance,
            InstrumentId = new Guid(source.InstrumentId),
        };

    private static PortfolioPosition[] Convert(
        this Position[] source,
        Guid parentSnapshotId)
        => [.. source.Select(t => t.Convert(parentSnapshotId))];
}
