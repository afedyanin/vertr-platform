using Vertr.PortfolioManager.Application.Entities;

namespace Vertr.PortfolioManager.Application.Abstractions;

public interface IOperationEventRepository
{
    public Task<OperationEvent[]> GetAll(string accountId, Guid? bookId = null, int maxRecords = 1000);

    public Task<bool> Save(OperationEvent[] operationEvents);
}
