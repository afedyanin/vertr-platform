using Microsoft.EntityFrameworkCore;
using Vertr.OrderExecution.Contracts;
using Vertr.OrderExecution.Contracts.Interfaces;
using Vertr.PortfolioManager.Contracts;

namespace Vertr.OrderExecution.DataAccess.Repositories;
internal class OrderEventRepository : RepositoryBase, IOrderEventRepository
{
    public OrderEventRepository(
        IDbContextFactory<OrderExecutionDbContext> contextFactory)
        : base(contextFactory)
    {
    }

    public async Task<PortfolioIdentity?> ResolvePortfolioByOrderId(string orderId)
    {
        using var context = await GetDbContext();

        var orderEvents = context.OrderEvents
            .Where(e => e.OrderId != null && e.OrderId == orderId);

        var accountId = orderEvents.FirstOrDefault(e => e.AccountId != null)?.AccountId;
        var subAccountId = orderEvents.FirstOrDefault(e => e.SubAccountId != Guid.Empty)?.SubAccountId;

        if (string.IsNullOrEmpty(accountId))
        {
            return null;
        }

        return new PortfolioIdentity(accountId, subAccountId);
    }

    public async Task<bool> Save(OrderEvent orderEvent)
    {
        using var context = await GetDbContext();
        await context.OrderEvents.AddAsync(orderEvent);
        var savedRecords = await context.SaveChangesAsync();
        return savedRecords > 0;
    }
}
