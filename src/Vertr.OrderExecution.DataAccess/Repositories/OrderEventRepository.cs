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

    public async Task<OrderEvent[]> GetAll(int limit = 1000)
    {
        using var context = await GetDbContext();

        var orderEvents = await context.OrderEvents
            .OrderByDescending(e => e.CreatedAt)
            .ThenBy(e => e.RequestId)
            .Take(limit)
            .ToArrayAsync();

        return orderEvents;
    }

    public async Task<OrderEvent?> GetById(Guid id)
    {
        using var context = await GetDbContext();

        var orderEvent = context.OrderEvents
            .SingleOrDefault(e => e.Id == id);

        return orderEvent;
    }

    public async Task<OrderEvent[]> GetBySubAccountId(Guid subAccountId)
    {
        using var context = await GetDbContext();

        var orderEvents = await context.OrderEvents
            .Where(e => e.SubAccountId == subAccountId)
            .OrderByDescending(e => e.CreatedAt)
            .ThenBy(e => e.RequestId)
            .ToArrayAsync();

        return orderEvents;
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

    public async Task<PortfolioIdentity?> ResolvePortfolioByOrderRequestId(Guid orderRequestId)
    {
        using var context = await GetDbContext();

        var orderEvents = context.OrderEvents
            .Where(e => e.RequestId != null && e.RequestId == orderRequestId);

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
