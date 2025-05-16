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

    public async Task<bool> Save(OrderEvent[] events)
    {
        using var context = await GetDbContext();
        await context.Portfolios.AddRangeAsync(events);
        var savedRecords = await context.SaveChangesAsync();
        return savedRecords > 0;
    }
}
