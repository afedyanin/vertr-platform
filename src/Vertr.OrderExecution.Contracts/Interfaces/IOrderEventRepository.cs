using Vertr.PortfolioManager.Contracts;

namespace Vertr.OrderExecution.Contracts.Interfaces;

public interface IOrderEventRepository
{
    public Task<OrderEvent[]> GetAll(int limit = 1000);

    public Task<OrderEvent[]> GetBySubAccountId(Guid subAccountId);

    public Task<OrderEvent?> GetById(Guid id);

    public Task<bool> Save(OrderEvent orderEvent);

    public Task<PortfolioIdentity?> ResolvePortfolioByOrderId(string orderId);

    public Task<PortfolioIdentity?> ResolvePortfolioByOrderRequestId(Guid orderRequestId);
}
