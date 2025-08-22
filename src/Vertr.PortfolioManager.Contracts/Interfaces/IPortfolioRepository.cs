namespace Vertr.PortfolioManager.Contracts.Interfaces;
public interface IPortfolioRepository
{
    public Task<Portfolio[]> GetAll();

    public Task<Portfolio?> GetById(Guid portfolioId);

    public Task<bool> Save(Portfolio portfolio);

    public Task<int> Delete(Guid portfolioId);
}
