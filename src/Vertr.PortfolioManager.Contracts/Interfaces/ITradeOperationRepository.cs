namespace Vertr.PortfolioManager.Contracts.Interfaces;

public interface ITradeOperationRepository
{
    public Task<TradeOperation[]> GetAll();

    public Task<TradeOperation[]> GetByPortfolio(PortfolioIdentity portfolioIdentity);

    public Task<bool> Save(TradeOperation[] tradeOoperations);

    public Task<int> DeleteAll(PortfolioIdentity portfolioIdentity);
}
