using Vertr.OrderExecution.Contracts;

namespace Vertr.OrderExecution.Application.Abstractions;
public interface IOrderExecutionService
{
    public Task<string?> PostMarketOrder(
        Guid requestId,
        Guid instrumentId,
        long qtyLots,
        PortfolioIdentity portfolioId);
}
