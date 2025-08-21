namespace Vertr.OrderExecution.Contracts.Interfaces;

public interface IOrderEventRepository
{
    public Task<OrderEvent[]> GetAll(int limit = 1000);

    public Task<OrderEvent[]> GetBySubAccountId(Guid subAccountId);

    public Task<OrderEvent?> GetById(Guid id);

    public Task<bool> Save(OrderEvent orderEvent);

    public Task<Guid?> ResolvePortfolioIdByOrderId(string orderId);

    public Task<Guid?> ResolvePortfolioIdByOrderRequestId(Guid orderRequestId);
}
