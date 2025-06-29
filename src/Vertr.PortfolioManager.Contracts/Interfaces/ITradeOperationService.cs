namespace Vertr.PortfolioManager.Contracts.Interfaces;

public interface ITradeOperationService
{
    public Task<Portfolio> ApplyOperation(TradeOperation operation);
}
