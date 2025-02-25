namespace Vertr.Domain;

public record class PortfolioSnapshot
{
    public DateTime TimeUtc { get; set; }

    public string AccountId { get; set; }

    public decimal TotalAmountShares { get; set; }

    public decimal TotalAmountBonds { get; set; }

    public decimal TotalAmountEtf { get; set; }

    public decimal TotalAmountCurrencies { get; set; }

    public decimal TotalAmountFutures { get; set; }

    public decimal TotalAmountOptions { get; set; }

    public decimal TotalAmountSp { get; set; }

    public decimal TotalAmountPortfolio { get; set; }

    public decimal ExpectedYield { get; set; }

    public IEnumerable<PortfolioPosition> Positions { get; set; } = [];
}
