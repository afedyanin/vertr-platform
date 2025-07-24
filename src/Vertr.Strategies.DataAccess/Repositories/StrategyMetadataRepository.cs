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
            .OrderByDescending(x => x.UpdatedAt)
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
        await context.Strategies.AddAsync(metadata);
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
