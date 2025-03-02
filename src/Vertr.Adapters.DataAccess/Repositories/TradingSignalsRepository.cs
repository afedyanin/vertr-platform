using Microsoft.EntityFrameworkCore;
using Npgsql;
using Vertr.Domain;
using Vertr.Domain.Repositories;
using Vertr.Domain.Settings;

namespace Vertr.Adapters.DataAccess.Repositories;
internal class TradingSignalsRepository : RepositoryBase, ITradingSignalsRepository
{
    public TradingSignalsRepository(
        IDbContextFactory<VertrDbContext> contextFactory) : base(contextFactory)
    {
    }
    public async Task<TradingSignal?> GetById(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        using var context = await GetDbContext();

        var item = await context
            .TradingSignals
            .AsNoTracking()
            .SingleOrDefaultAsync(x => x.Id == id, cancellationToken);

        return item;
    }

    public async Task<TradingSignal?> GetLast(
        StrategySettings strategySettings,
        CancellationToken cancellationToken = default)
    {
        using var context = await GetDbContext();

        var query = context
            .TradingSignals
            .AsNoTracking()
            .Where(x =>
                x.Symbol == strategySettings.Symbol &&
                x.CandleInterval == strategySettings.Interval &&
                x.PredictorType == strategySettings.PredictorType &&
                x.Sb3Algo == strategySettings.Sb3Algo);

        var last = await query
            .OrderByDescending(x => x.TimeUtc)
            .FirstOrDefaultAsync(cancellationToken);

        return last;
    }

    public async Task<int> Insert(
        TradingSignal signal,
        CancellationToken cancellationToken = default)
    {
        try
        {
            using var context = await GetDbContext();

            var entry = await context.TradingSignals.AddAsync(signal, cancellationToken);
            var savedRecords = await context.SaveChangesAsync(cancellationToken);

            return savedRecords;
        }
        catch (DbUpdateException ex)
        {
            if (ex.InnerException is PostgresException pgex)
            {
                // duplicate key value violates unique constraint "tinvest_operations_pkey"
                if (pgex.SqlState == "23505")
                {
                    // ignore insert duplicates
                    return 0;
                }
            }

            throw;
        }
    }

    public async Task<int> Delete(
        Guid tradingSignalId,
        CancellationToken cancellationToken = default)
    {
        using var context = await GetDbContext();

        var count = await context
            .TradingSignals
            .Where(s => s.Id == tradingSignalId)
            .ExecuteDeleteAsync(cancellationToken);

        return count;
    }
}
