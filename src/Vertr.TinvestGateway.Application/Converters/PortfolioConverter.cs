using Vertr.PortfolioManager.Contracts;
using Vertr.TinvestGateway.Contracts;

namespace Vertr.TinvestGateway.Application.Converters;

internal static class PortfolioConverter
{
    public static PortfolioSnapshot? Convert(
        this Tinkoff.InvestApi.V1.PortfolioResponse source)
    {
        if (source == null)
        {
            return null;
        }

        var response = source.ConvertToResponse();

        var res = new PortfolioSnapshot
        {
            UpdatedAt = DateTime.UtcNow,
            AccountId = response.AccountId,
            BookId = Guid.Empty,
            Positions = response.Positions.Convert()
        };

        return res;
    }

    private static PortfolioResponse ConvertToResponse(
        this Tinkoff.InvestApi.V1.PortfolioResponse source)
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

    private static PortfolioPosition Convert(this Position source)
        => new PortfolioPosition
        {
            Balance = source.Balance,
            InstrumentId = new Guid(source.InstrumentId),
        };

    private static PortfolioPosition[] Convert(
        this Position[] source)
        => [.. source.Select(t => t.Convert())];
}
