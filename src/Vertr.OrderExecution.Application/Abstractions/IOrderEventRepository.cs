using Vertr.OrderExecution.Application.Entities;
using Vertr.OrderExecution.Contracts;
using Vertr.PortfolioManager.Contracts;

namespace Vertr.OrderExecution.Application.Abstractions;

public interface IOrderEventRepository
{
    public Task<bool> Save(OrderEvent orderEvent);

    public Task<PortfolioIdentity> GetPortfolioIdByOrderId(string orderId);
}
