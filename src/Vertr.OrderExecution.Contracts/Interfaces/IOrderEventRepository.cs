namespace Vertr.OrderExecution.Contracts.Interfaces;

public interface IOrderEventRepository
{
    public Task<bool> Save(OrderEvent orderEvent);

    public Task<PortfolioIdentity?> ResolvePortfolioByOrderId(string orderId);
}
