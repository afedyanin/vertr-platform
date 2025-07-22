namespace Vertr.PortfolioManager.Contracts.Interfaces;

public interface ITradeOperationRepository
{
    public Task<TradeOperation[]> GetAll();

    public Task<TradeOperation[]> GetByPortfolio(PortfolioIdentity portfolioIdentity);

    public Task<TradeOperation[]> GetByAccountId(string accountId);

    public Task<bool> Save(TradeOperation tradeOoperation);

    public Task<int> Delete(PortfolioIdentity portfolioIdentity);

    public Task<int> DeleteByAccountId(string accountId);
}
