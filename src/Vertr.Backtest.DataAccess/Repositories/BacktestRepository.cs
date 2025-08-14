using Microsoft.EntityFrameworkCore;
using Vertr.Backtest.Contracts;
using Vertr.Backtest.Contracts.Interfaces;

namespace Vertr.Backtest.DataAccess.Repositories;
internal class BacktestRepository : RepositoryBase, IBacktestRepository
{
    public BacktestRepository(IDbContextFactory<BacktestDbContext> contextFactory) : base(contextFactory)
    {
    }

    public async Task<BacktestRun[]> GetAll()
    {
        using var context = await GetDbContext();

        return await context
            .Backtests
            .OrderByDescending(x => x.CreatedAt)
            .ToArrayAsync();
    }

    public async Task<BacktestRun?> GetById(Guid id)
    {
        using var context = await GetDbContext();

        return await context
            .Backtests
            .SingleOrDefaultAsync(x => x.Id == id);
    }

    public async Task<bool> Save(BacktestRun backtest)
    {
        using var context = await GetDbContext();

        var existing = await context
            .Backtests
            .FirstOrDefaultAsync(p => p.Id == backtest.Id);

        if (existing != null)
        {
            existing.From = backtest.From;
            existing.To = backtest.To;
            existing.CreatedAt = backtest.CreatedAt;
            existing.Description = backtest.Description;
            existing.StrategyId = backtest.StrategyId;
            existing.SubAccountId = backtest.SubAccountId;
            existing.ExecutionState = backtest.ExecutionState;
            existing.UpdatedAt = backtest.UpdatedAt;
            existing.ProgressMessage = backtest.ProgressMessage;
            existing.IsCancellationRequested = backtest.IsCancellationRequested;
        }
        else
        {
            context.Backtests.Add(backtest);
        }

        var savedRecords = await context.SaveChangesAsync();

        return savedRecords > 0;
    }

    public async Task<bool> Cancel(Guid id)
    {
        using var context = await GetDbContext();

        var existing = await context
            .Backtests
            .FirstOrDefaultAsync(p => p.Id == id);

        if (existing == null)
        {
            return false;
        }

        existing.IsCancellationRequested = true;
        var savedRecords = await context.SaveChangesAsync();

        return savedRecords > 0;
    }

    public async Task<int> Delete(Guid Id)
    {
        using var context = await GetDbContext();

        return await context.Backtests
            .Where(s => s.Id == Id)
            .ExecuteDeleteAsync();
    }
}
