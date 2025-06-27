namespace Vertr.PortfolioManager.Contracts.Interfaces;
public interface IPortfolioRepository
{
    public Task<PortfolioSnapshot?> GetPortfolio(PortfolioIdentity portfolioIdentity);

    public Task SetPortfolio(PortfolioSnapshot portfolio);

    public Task Remove(PortfolioIdentity portfolioIdentity);
}
