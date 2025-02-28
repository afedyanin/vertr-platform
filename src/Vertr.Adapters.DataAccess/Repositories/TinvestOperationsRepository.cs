using Microsoft.EntityFrameworkCore;
using Npgsql;
using Vertr.Domain;
using Vertr.Domain.Repositories;

namespace Vertr.Adapters.DataAccess.Repositories;

internal class TinvestOperationsRepository : RepositoryBase, ITinvestOperationsRepository
{
    public TinvestOperationsRepository(IDbContextFactory<VertrDbContext> contextFactory) : base(contextFactory)
    {
    }

    public async Task<Operation?> GetById(Guid id, CancellationToken cancellationToken = default)
    {
        using var context = await GetDbContext();

        var operation = await context
            .TinvestOperations
            .AsNoTracking()
            .SingleOrDefaultAsync(x => x.Id == id, cancellationToken);

        return operation;
    }

    public async Task<IEnumerable<Operation>> Get(
        string accountId,
        DateTime? from = null,
        DateTime? to = null,
        CancellationToken cancellationToken = default)
    {
        using var context = await GetDbContext();

        var query = context
            .TinvestOperations
            .AsNoTracking()
            .Where(x => x.AccountId == accountId);

        if (from.HasValue)
        {
            query = query.Where(x => x.Date >= from);
        };

        if (to.HasValue)
        {
            query = query.Where(x => x.Date <= to);
        };

        var snapshots = await query.ToArrayAsync(cancellationToken);

        return snapshots;
    }

    public async Task<Operation?> GetLast(
        string accountId,
        CancellationToken cancellationToken = default)
    {
        using var context = await GetDbContext();

        var last = await context
            .TinvestOperations
            .AsNoTracking()
            .Where(x => x.AccountId == accountId)
            .OrderByDescending(x => x.Date)
            .FirstOrDefaultAsync(cancellationToken);

        return last;
    }

    public async Task<int> Update(Operation operation, CancellationToken cancellationToken = default)
    {
        using var context = await GetDbContext();

        var entry = context.TinvestOperations.Update(operation);
        var savedRecords = await context.SaveChangesAsync(cancellationToken);

        return savedRecords;
    }

    public async Task<int> Insert(Operation operation, CancellationToken cancellationToken = default)
    {
        try
        {
            using var context = await GetDbContext();

            var entry = await context.TinvestOperations.AddAsync(operation, cancellationToken);
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

    public async Task<int> Delete(Guid operationId, CancellationToken cancellationToken = default)
    {
        using var context = await GetDbContext();
        var count = await context.TinvestOperations.Where(s => s.Id == operationId).ExecuteDeleteAsync(cancellationToken);

        return count;
    }
}
