using Microsoft.EntityFrameworkCore;
using Vertr.PortfolioManager.Application.Abstractions;
using Vertr.PortfolioManager.Application.Entities;

namespace Vertr.PortfolioManager.DataAccess.Repositories;
internal class OperationEventRepository : RepositoryBase, IOperationEventRepository
{
    public OperationEventRepository(
        IDbContextFactory<PortfolioDbContext> contextFactory) : base(contextFactory)
    {
    }

    public async Task<OperationEvent[]> GetAll(string accountId, Guid? bookId = null, int maxRecords = 1000)
    {
        using var context = await GetDbContext();

        var snapshot = await context
            .Operations
            .AsNoTracking()
            .Where(x =>
                x.AccountId == accountId &&
                ((bookId.HasValue && x.BookId == bookId.Value) || x.BookId == null))
            .OrderByDescending(x => x.CreatedAt)
            .Take(maxRecords)
            .ToArrayAsync();

        return snapshot;
    }

    public async Task<bool> Save(OperationEvent[] operationEvents)
    {
        using var context = await GetDbContext();
        await context.Operations.AddRangeAsync(operationEvents);
        var savedRecords = await context.SaveChangesAsync();

        return savedRecords > 0;
    }
}
