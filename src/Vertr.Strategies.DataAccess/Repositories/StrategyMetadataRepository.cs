using Microsoft.EntityFrameworkCore;
using Vertr.Strategies.Contracts;
using Vertr.Strategies.Contracts.Interfaces;

namespace Vertr.Strategies.DataAccess.Repositories;

internal class StrategyMetadataRepository : RepositoryBase, IStrategyMetadataRepository
{
    public StrategyMetadataRepository(
        IDbContextFactory<StrategiesDbContext> contextFactory)
        : base(contextFactory)
    {
    }

    public async Task<StrategyMetadata[]> GetAll()
    {
        using var context = await GetDbContext();

        return await context
            .Strategies
            .OrderByDescending(x => x.CreatedAt)
            .ToArrayAsync();
    }

    public async Task<StrategyMetadata?> GetById(Guid id)
    {
        using var context = await GetDbContext();

        return await context
            .Strategies
            .SingleOrDefaultAsync(x => x.Id == id);
    }

    public async Task<bool> Save(StrategyMetadata metadata)
    {
        using var context = await GetDbContext();

        var existing = await context
            .Strategies
            .FirstOrDefaultAsync(p => p.Id == metadata.Id);

        if (existing != null)
        {
            existing.Name = metadata.Name;
            existing.Type = metadata.Type;
            existing.InstrumentId = metadata.InstrumentId;
            existing.SubAccountId = metadata.SubAccountId;
            existing.BacktestId = metadata.BacktestId;
            existing.QtyLots = metadata.QtyLots;
            existing.IsActive = metadata.IsActive;
            existing.ParamsJson = metadata.ParamsJson;
            existing.CreatedAt = metadata.CreatedAt;
        }
        else
        {
            context.Strategies.Add(metadata);
        }

        var savedRecords = await context.SaveChangesAsync();

        return savedRecords > 0;
    }

    public async Task<int> Delete(Guid Id)
    {
        using var context = await GetDbContext();

        return await context.Strategies
            .Where(s => s.Id == Id)
            .ExecuteDeleteAsync();
    }
}
