using Microsoft.EntityFrameworkCore;
using Vertr.MarketData.Contracts;
using Vertr.MarketData.Contracts.Interfaces;

namespace Vertr.MarketData.DataAccess.Repositories;

internal class SubscriptionsRepository : RepositoryBase, ISubscriptionsRepository
{
    public SubscriptionsRepository(IDbContextFactory<MarketDataDbContext> contextFactory) : base(contextFactory)
    {
    }

    public async Task<CandleSubscription[]> GetAll()
    {
        using var context = await GetDbContext();

        return await context
            .CandleSubscriptions
            .ToArrayAsync();
    }

    public async Task<CandleSubscription?> GetById(Guid id)
    {
        using var context = await GetDbContext();

        return await context
            .CandleSubscriptions
            .SingleOrDefaultAsync(x => x.Id == id);
    }

    public async Task<bool> Save(CandleSubscription subscription)
    {
        using var context = await GetDbContext();

        var existing = await context
            .CandleSubscriptions
            .FirstOrDefaultAsync(p => p.Id == subscription.Id);

        if (existing != null)
        {
            existing.InstrumentId = subscription.InstrumentId;
            existing.Interval = subscription.Interval;
            existing.ExternalStatus = subscription.ExternalStatus;
            existing.ExternalSubscriptionId = subscription.ExternalSubscriptionId;
            existing.Disabled = subscription.Disabled;
            existing.LoadHistory = subscription.LoadHistory;
        }
        else
        {
            context.CandleSubscriptions.Add(subscription);
        }

        var savedRecords = await context.SaveChangesAsync();

        return savedRecords > 0;
    }

    public async Task<int> Delete(Guid Id)
    {
        using var context = await GetDbContext();

        return await context.CandleSubscriptions
            .Where(s => s.Id == Id)
            .ExecuteDeleteAsync();
    }

}
