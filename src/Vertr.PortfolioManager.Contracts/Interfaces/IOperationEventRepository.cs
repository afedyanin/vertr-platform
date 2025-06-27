using Vertr.OrderExecution.Contracts;

namespace Vertr.PortfolioManager.Contracts.Interfaces;

public interface IOperationEventRepository
{
    public Task<OperationEvent[]> GetAll(PortfolioIdentity portfolioIdentity, int maxRecords = 1000);

    public Task<bool> Save(OperationEvent[] operationEvents);

    public Task<int> DeleteAll(PortfolioIdentity portfolioIdentity);
}
