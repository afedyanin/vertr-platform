namespace Vertr.PortfolioManager.Contracts.Interfaces;
public interface IPortfolioProvider
{
    public string[] GetActiveAccounts();

    public Portfolio[] GetAllPortfolios();

    public Portfolio? GetPortfolio(PortfolioIdentity portfolioIdentity);

    public Position? GetPosition(PortfolioIdentity portfolioIdentity, Guid instrumentId);

    public void Update(Portfolio portfolio);

    public void Remove(PortfolioIdentity portfolioIdentity);
}
