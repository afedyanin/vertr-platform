using Microsoft.EntityFrameworkCore;
using Vertr.Domain;
using Vertr.Domain.Repositories;

namespace Vertr.Adapters.DataAccess.Repositories;
internal class TinvestPortfolioRepository : RepositoryBase, ITinvestPortfolioRepository
{
    public TinvestPortfolioRepository(IDbContextFactory<VertrDbContext> contextFactory) : base(contextFactory)
    {
    }

    public async Task<PortfolioSnapshot?> GetById(Guid id, CancellationToken cancellationToken = default)
    {
        using var context = await GetDbContext();

        var snapshot = await context
            .TinvestPortfolios
            .Include(s => s.Positions)
            .AsNoTracking()
            .SingleOrDefaultAsync(x => x.Id == id, cancellationToken);

        return snapshot;
    }

    public async Task<IEnumerable<PortfolioSnapshot>> Get(
        string accountId,
        DateTime? from = null,
        DateTime? to = null,
        CancellationToken cancellationToken = default)
    {
        using var context = await GetDbContext();

        var query = context
            .TinvestPortfolios
            .AsNoTracking()
            .Where(x => x.AccountId == accountId);

        if (from.HasValue)
        {
            query = query.Where(x => x.TimeUtc >= from);
        };

        if (to.HasValue)
        {
            query = query.Where(x => x.TimeUtc <= to);
        };

        var snapshots = await query.ToArrayAsync(cancellationToken);

        return snapshots;
    }

    public async Task<int> Update(PortfolioSnapshot snapshot, CancellationToken cancellationToken = default)
    {
        using var context = await GetDbContext();

        var entry = context.TinvestPortfolios.Update(snapshot);
        var savedRecords = await context.SaveChangesAsync(cancellationToken);

        return savedRecords;
    }

    public async Task<int> Insert(PortfolioSnapshot snapshot, CancellationToken cancellationToken = default)
    {
        using var context = await GetDbContext();

        var entry = await context.TinvestPortfolios.AddAsync(snapshot, cancellationToken);
        var savedRecords = await context.SaveChangesAsync(cancellationToken);

        return savedRecords;
    }

    public async Task<int> Delete(Guid snapshotId, CancellationToken cancellationToken = default)
    {
        using var context = await GetDbContext();
        var count = await context.TinvestPortfolios.Where(s => s.Id == snapshotId).ExecuteDeleteAsync(cancellationToken);

        return count;
    }
}
