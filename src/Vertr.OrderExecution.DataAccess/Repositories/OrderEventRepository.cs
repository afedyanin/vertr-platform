using Microsoft.EntityFrameworkCore;
using Vertr.OrderExecution.Contracts;
using Vertr.OrderExecution.Contracts.Interfaces;

namespace Vertr.OrderExecution.DataAccess.Repositories;
internal class OrderEventRepository : RepositoryBase, IOrderEventRepository
{
    private readonly IOrderEventHandler _orderEventHandler;
    public OrderEventRepository(
        IOrderEventHandler orderEventHandler,
        IDbContextFactory<OrderExecutionDbContext> contextFactory)
        : base(contextFactory)
    {
        _orderEventHandler = orderEventHandler;
    }

    public async Task<OrderEvent[]> GetAll(int limit = 1000)
    {
        using var context = await GetDbContext();

        var orderEvents = await context.OrderEvents
            .OrderByDescending(e => e.CreatedAt)
            .ThenByDescending(e => e.RequestId)
            .ThenBy(e => e.OrderId)
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

    public async Task<OrderEvent[]> GetByPortfolioId(Guid portfolioId, int limit = 1000)
    {
        using var context = await GetDbContext();

        var orderEvents = await context.OrderEvents
            .Where(e => e.PortfolioId == portfolioId)
            .OrderByDescending(e => e.CreatedAt)
            .ThenBy(e => e.RequestId)
            .Take(limit)
            .ToArrayAsync();

        return orderEvents;
    }

    public async Task<Guid?> ResolvePortfolioIdByOrderId(string orderId)
    {
        using var context = await GetDbContext();

        var orderEvents = context.OrderEvents
            .Where(e => e.OrderId != null && e.OrderId == orderId);

        var portfolioId = orderEvents.FirstOrDefault(e => e.PortfolioId != Guid.Empty)?.PortfolioId;

        return portfolioId;
    }

    public async Task<Guid?> ResolvePortfolioIdByOrderRequestId(Guid orderRequestId)
    {
        using var context = await GetDbContext();

        var orderEvents = context.OrderEvents
            .Where(e => e.RequestId != null && e.RequestId == orderRequestId);

        var portfolioId = orderEvents.FirstOrDefault(e => e.PortfolioId != Guid.Empty)?.PortfolioId;

        return portfolioId;
    }

    public async Task<bool> Save(OrderEvent orderEvent)
    {
        using var context = await GetDbContext();
        await context.OrderEvents.AddAsync(orderEvent);
        var savedRecords = await context.SaveChangesAsync();

        if (savedRecords > 0)
        {
            await _orderEventHandler.HandleOrderEvent(orderEvent);
            return true;
        }

        return false;
    }

    public async Task<int> DeleteByPortfolioId(Guid portfolioId)
    {
        using var context = await GetDbContext();

        return await context.OrderEvents
            .Where(s => s.PortfolioId == portfolioId)
            .ExecuteDeleteAsync();
    }
}
