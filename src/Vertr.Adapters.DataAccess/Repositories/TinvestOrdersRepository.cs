using Microsoft.EntityFrameworkCore;
using Npgsql;
using Vertr.Domain;
using Vertr.Domain.Repositories;

namespace Vertr.Adapters.DataAccess.Repositories;
internal class TinvestOrdersRepository : RepositoryBase, ITinvestOrdersRepository
{
    public TinvestOrdersRepository(IDbContextFactory<VertrDbContext> contextFactory) : base(contextFactory)
    {
    }

    public async Task<PostOrderResponse?> GetById(Guid id, CancellationToken cancellationToken = default)
    {
        using var context = await GetDbContext();

        var orderResponse = await context
            .TinvestOrders
            .AsNoTracking()
            .SingleOrDefaultAsync(x => x.Id == id, cancellationToken);

        return orderResponse;
    }

    public async Task<IEnumerable<PostOrderResponse>> Get(
        string accountId,
        DateTime? from = null,
        DateTime? to = null,
        CancellationToken cancellationToken = default)
    {
        using var context = await GetDbContext();

        var query = context
            .TinvestOrders
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

        var orders = await query.ToArrayAsync(cancellationToken);

        return orders;
    }

    public async Task<PostOrderResponse?> GetLast(
        string accountId,
        CancellationToken cancellationToken = default)
    {
        using var context = await GetDbContext();

        var last = await context
            .TinvestOrders
            .AsNoTracking()
            .Where(x => x.AccountId == accountId)
            .OrderByDescending(x => x.TimeUtc)
            .FirstOrDefaultAsync(cancellationToken);

        return last;
    }

    public async Task<int> Update(PostOrderResponse order, CancellationToken cancellationToken = default)
    {
        using var context = await GetDbContext();

        var entry = context.TinvestOrders.Update(order);
        var savedRecords = await context.SaveChangesAsync(cancellationToken);

        return savedRecords;
    }

    public async Task<int> Insert(PostOrderResponse order, CancellationToken cancellationToken = default)
    {
        try
        {
            using var context = await GetDbContext();

            var entry = await context.TinvestOrders.AddAsync(order, cancellationToken);
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

    public async Task<int> Delete(Guid orderResponseId, CancellationToken cancellationToken = default)
    {
        using var context = await GetDbContext();
        var count = await context.TinvestOrders.Where(s => s.Id == orderResponseId).ExecuteDeleteAsync(cancellationToken);

        return count;
    }
}
