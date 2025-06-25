using Vertr.OrderExecution.Contracts;
using Vertr.PortfolioManager.Application.Entities;

namespace Vertr.PortfolioManager.Application.Abstractions;

public interface IOperationEventRepository
{
    public Task<OperationEvent[]> GetAll(PortfolioIdentity portfolioIdentity, int maxRecords = 1000);

    public Task<bool> Save(OperationEvent[] operationEvents);

    public Task<int> Delete(PortfolioIdentity portfolioIdentity);
}
