using Microsoft.EntityFrameworkCore;
using Vertr.PortfolioManager.Application.Abstractions;
using Vertr.PortfolioManager.Contracts;

namespace Vertr.PortfolioManager.DataAccess.Repositories;
internal class PortfolioSnapshotRepository : RepositoryBase, IPortfolioSnapshotRepository
{
    public PortfolioSnapshotRepository(
        IDbContextFactory<PortfolioDbContext> contextFactory) : base(contextFactory)
    {
    }

    public async Task<PortfolioSnapshot?> GetLast(string accountId, Guid? bookId = null)
    {
        using var context = await GetDbContext();

        var snapshot = await context
            .Portfolios
            .Include(s => s.Positions)
            .AsNoTracking()
            .Where(x =>
                x.AccountId == accountId &&
                ((bookId.HasValue && x.BookId == bookId.Value) || x.BookId == null))
            .OrderByDescending(x => x.UpdatedAt)
            .FirstOrDefaultAsync();

        return snapshot;
    }

    public async Task<PortfolioSnapshot[]> GetHistory(string accountId, Guid? bookId = null, int maxRecords = 100)
    {
        using var context = await GetDbContext();

        var snapshot = await context
            .Portfolios
            .Include(s => s.Positions)
            .AsNoTracking()
            .Where(x =>
                x.AccountId == accountId &&
                ((bookId.HasValue && x.BookId == bookId.Value) || x.BookId == null))
            .OrderByDescending(x => x.UpdatedAt)
            .Take(maxRecords)
            .ToArrayAsync();

        return snapshot;
    }

    public async Task<bool> Save(PortfolioSnapshot portfolio)
    {
        using var context = await GetDbContext();

        var entry = await context.Portfolios.AddAsync(portfolio);
        var savedRecords = await context.SaveChangesAsync();

        return savedRecords > 0;
    }

    public async Task<bool> Delete(Guid id)
    {
        using var context = await GetDbContext();
        var count = await context.Portfolios
            .Where(s => s.Id == id)
            .ExecuteDeleteAsync();

        return count > 0;
    }

    public async Task<int> DeleteByAccountId(string accountId)
    {
        using var context = await GetDbContext();

        var count = await context.Portfolios
            .Where(s => s.AccountId == accountId)
            .ExecuteDeleteAsync();

        return count;
    }

    public async Task<int> DeleteByBookId(Guid bookId)
    {
        using var context = await GetDbContext();

        var count = await context.Portfolios
            .Where(s => s.BookId == bookId)
            .ExecuteDeleteAsync();

        return count;
    }
}
