using Microsoft.EntityFrameworkCore;
using Vertr.Strategies.Contracts;
using Vertr.Strategies.Contracts.Interfaces;

namespace Vertr.Strategies.DataAccess.Repositories;

internal class TradingSignalRepository : RepositoryBase, ITradingSignalRepository
{
    public TradingSignalRepository(IDbContextFactory<StrategiesDbContext> contextFactory) : base(contextFactory)
    {
    }

    public async Task<TradingSignal[]> GetAll()
    {
        using var context = await GetDbContext();

        return await context
            .TradingSignals
            .OrderByDescending(x => x.CreatedAt)
            .ToArrayAsync();
    }

    public async Task<TradingSignal?> GetById(Guid id)
    {
        using var context = await GetDbContext();

        return await context
            .TradingSignals
            .SingleOrDefaultAsync(x => x.Id == id);
    }

    public async Task<bool> Save(TradingSignal signal)
    {
        using var context = await GetDbContext();
        context.TradingSignals.Add(signal);
        var savedRecords = await context.SaveChangesAsync();

        return savedRecords > 0;
    }
    public async Task<int> Delete(Guid Id)
    {
        using var context = await GetDbContext();

        return await context.TradingSignals
            .Where(s => s.Id == Id)
            .ExecuteDeleteAsync();
    }
}
