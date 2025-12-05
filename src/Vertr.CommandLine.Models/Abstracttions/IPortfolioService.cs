namespace Vertr.CommandLine.Models.Abstracttions;

public interface IPortfolioService
{
    public Position[] Update(
        Guid portfolioId,
        string symbol,
        Trade[] trades,
        string currencyCode);

    public Position[] GetPositions(Guid portfolioId);

    public Position? GetPosition(Guid portfolioId, string symbol);
}