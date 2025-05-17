using Vertr.OrderExecution.Application.Entities;

namespace Vertr.OrderExecution.Application.Abstractions;

public interface IOrderEventRepository
{
    public Task<bool> Save(OrderEvent orderEvent);

    public Task<Guid?> GetBookIdByOrderId(string orderId);

    public Task<string?> GetAccountIdByOrderId(string orderId);
}
