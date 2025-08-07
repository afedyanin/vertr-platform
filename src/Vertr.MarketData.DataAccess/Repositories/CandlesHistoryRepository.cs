using Microsoft.EntityFrameworkCore;
using Vertr.MarketData.Contracts;
using Vertr.MarketData.Contracts.Interfaces;

namespace Vertr.MarketData.DataAccess.Repositories;

internal class CandlesHistoryRepository : RepositoryBase, ICandlesHistoryRepository
{
    public CandlesHistoryRepository(IDbContextFactory<MarketDataDbContext> contextFactory) : base(contextFactory)
    {
    }

    public async Task<CandlesHistoryItem[]> Get(Guid instrumentId, DateOnly? from = null, DateOnly? to = null)
    {
        using var context = await GetDbContext();

        var query = context.CandlesHistory.Where(x => x.Id == instrumentId);

        if (from != null)
        {
            query = query.Where(x => x.Day >= from);
        }

        if (to != null)
        {
            query = query.Where(x => x.Day <= to);
        }

        return await query.ToArrayAsync();
    }

    public async Task<CandlesHistoryItem?> GetById(Guid id)
    {
        using var context = await GetDbContext();

        return await context
            .CandlesHistory
            .SingleOrDefaultAsync(x => x.Id == id);
    }

    public async Task<bool> Save(CandlesHistoryItem item)
    {
        using var context = await GetDbContext();

        var existing = await context
            .CandlesHistory
            .FirstOrDefaultAsync(p => p.Id == item.Id);

        if (existing != null)
        {
            existing.InstrumentId = item.InstrumentId;
            existing.Interval = item.Interval;
            existing.Day = item.Day;
            existing.Data = item.Data;
            existing.Count = item.Count;
        }
        else
        {
            context.CandlesHistory.Add(item);
        }

        var savedRecords = await context.SaveChangesAsync();

        return savedRecords > 0;
    }

    public async Task<int> Delete(Guid Id)
    {
        using var context = await GetDbContext();

        return await context.CandlesHistory
            .Where(s => s.Id == Id)
            .ExecuteDeleteAsync();
    }

    public async Task<int> DeleteAll(Guid instrumentId)
    {
        using var context = await GetDbContext();

        return await context.CandlesHistory
            .Where(s => s.InstrumentId == instrumentId)
            .ExecuteDeleteAsync();
    }
}
