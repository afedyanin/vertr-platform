namespace Vertr.PortfolioManager.Contracts.Interfaces;
public interface IPortfolioRepository
{
    public string[] GetActiveAccounts();

    public Portfolio[] GetAllPortfolios();

    public Portfolio? GetPortfolio(PortfolioIdentity portfolioIdentity);

    public void Save(Portfolio portfolio);

    public void Delete(PortfolioIdentity portfolioIdentity);
}
