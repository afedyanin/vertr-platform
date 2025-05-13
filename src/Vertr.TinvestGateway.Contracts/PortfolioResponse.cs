namespace Vertr.TinvestGateway.Contracts;
public class PortfolioResponse
{
    public string AccountId { get; init; } = string.Empty;

    public decimal TotalAmountShares { get; init; }

    public decimal TotalAmountBonds { get; init; }

    public decimal TotalAmountEtf { get; init; }

    public decimal TotalAmountCurrencies { get; init; }

    public decimal TotalAmountFutures { get; init; }

    public decimal TotalAmountOptions { get; init; }

    public decimal TotalAmountSp { get; init; }

    public decimal TotalAmountPortfolio { get; init; }

    public decimal ExpectedYield { get; init; }

    public Position[] Positions { get; init; } = [];
}
