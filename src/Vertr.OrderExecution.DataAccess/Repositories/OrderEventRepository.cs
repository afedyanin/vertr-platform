using Microsoft.EntityFrameworkCore;
using Vertr.OrderExecution.Application.Abstractions;
using Vertr.OrderExecution.Application.Entities;

namespace Vertr.OrderExecution.DataAccess.Repositories;
internal class OrderEventRepository : RepositoryBase, IOrderEventRepository
{
    public OrderEventRepository(
        IDbContextFactory<OrderExecutionDbContext> contextFactory)
        : base(contextFactory)
    {
    }

    public async Task<(string?, Guid?)> GetAccountIdBookIdByOrderId(string orderId)
    {
        using var context = await GetDbContext();

        var orderEvents = context.OrderEvents
            .Where(
                e => e.OrderId != null &&
                e.OrderId.Equals(orderId, StringComparison.OrdinalIgnoreCase));

        var accountId = orderEvents.FirstOrDefault(e => e.AccountId != null);
        var bookId = orderEvents.FirstOrDefault(e => e.BookId != null);

        return (accountId, bookId);
    }

    public async Task<Guid?> GetBookIdByOrderId(string orderId)
    {
        using var context = await GetDbContext();

        var orderEvent = context.OrderEvents
            .Where(
                e => e.OrderId != null &&
                e.OrderId.Equals(orderId, StringComparison.OrdinalIgnoreCase) &&
                e.BookId != null)
            .FirstOrDefault();

        return orderEvent?.BookId;
    }

    public async Task<bool> Save(OrderEvent orderEvent)
    {
        using var context = await GetDbContext();
        await context.OrderEvents.AddAsync(orderEvent);
        var savedRecords = await context.SaveChangesAsync();
        return savedRecords > 0;
    }
}
