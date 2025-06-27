namespace Vertr.PortfolioManager.Contracts.Interfaces;

public interface ITradeOperationRepository
{
    public Task<TradeOperation[]> GetAll(PortfolioIdentity portfolioIdentity, int maxRecords = 1000);

    public Task<bool> Save(TradeOperation[] tradeOoperations);

    public Task<int> DeleteAll(PortfolioIdentity portfolioIdentity);
}
