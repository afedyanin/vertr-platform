namespace Vertr.PortfolioManager.Contracts.Interfaces;
public interface IPortfolioProvider
{
    public Portfolio[] GetAllPortfolios();

    public Portfolio? GetPortfolio(Guid portfolioId);

    public Position? GetPosition(Guid portfolioId, Guid instrumentId);

    public void Update(Portfolio portfolio);

    public void Remove(Guid portfolioId);
}
