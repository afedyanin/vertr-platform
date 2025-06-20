using Vertr.OrderExecution.Contracts;

namespace Vertr.TinvestGateway.Application.Converters;

internal static class PortfolioConverter
{
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
}
