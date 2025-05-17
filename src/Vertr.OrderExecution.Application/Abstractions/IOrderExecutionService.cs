using Vertr.OrderExecution.Application.Entities;

namespace Vertr.OrderExecution.Application.Abstractions;
public interface IOrderExecutionService
{
    public Task<PostOrderResult> PostMarketOrder(
        Guid requestId,
        Guid instrumentId,
        string accountId,
        long qtyLots,
        Guid bookId);
}
