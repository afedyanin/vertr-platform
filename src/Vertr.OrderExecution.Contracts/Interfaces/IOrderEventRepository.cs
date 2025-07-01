using Vertr.PortfolioManager.Contracts;

namespace Vertr.OrderExecution.Contracts.Interfaces;

public interface IOrderEventRepository
{
    public Task<bool> Save(OrderEvent orderEvent);

    public Task<PortfolioIdentity?> ResolvePortfolioByOrderId(string orderId);

    public Task<PortfolioIdentity?> ResolvePortfolioByOrderRequestId(Guid orderRequestId);
}
