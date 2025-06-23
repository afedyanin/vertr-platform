using Vertr.OrderExecution.Application.Entities;
using Vertr.OrderExecution.Contracts;

namespace Vertr.OrderExecution.Application.Abstractions;

public interface IOrderEventRepository
{
    public Task<bool> Save(OrderEvent orderEvent);

    public Task<PortfolioIdentity?> ResolvePortfolioByOrderId(string orderId);
}
