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

    public static PortfolioResponse Convert(this Tinkoff.InvestApi.V1.PortfolioResponse source)
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

    public static PortfolioSnapshot? Convert(
        this PortfolioResponse? source,
        Guid? bookId)
    {
        if (source == null)
        {
            return null;
        }

        var res = new PortfolioSnapshot
        {
            Id = Guid.NewGuid(),
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
        this Position source,
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
        this Position[] source,
        PortfolioSnapshot parent)
        => [.. source.Select(t => t.Convert(parent))];
}
