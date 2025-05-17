namespace Vertr.OrderExecution.Application.Abstractions;
public interface IOrderExecutionService
{
    public Task<string?> PostMarketOrder(
        Guid requestId,
        Guid instrumentId,
        string accountId,
        long qtyLots,
        Guid bookId);
}
