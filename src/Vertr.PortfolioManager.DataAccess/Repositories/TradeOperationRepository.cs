using Microsoft.EntityFrameworkCore;
using Vertr.PortfolioManager.Contracts;
using Vertr.PortfolioManager.Contracts.Interfaces;

namespace Vertr.PortfolioManager.DataAccess.Repositories;
internal class TradeOperationRepository : RepositoryBase, ITradeOperationRepository
{
    public TradeOperationRepository(
        IDbContextFactory<PortfolioDbContext> contextFactory) : base(contextFactory)
    {
    }

    public async Task<TradeOperation[]> GetAll(PortfolioIdentity portfolioIdentity, int maxRecords = 1000)
    {
        using var context = await GetDbContext();

        if (!portfolioIdentity.BookId.HasValue)
        {
            return await context
                .Operations
                .AsNoTracking()
                .Where(x => x.AccountId == portfolioIdentity.AccountId)
                .OrderByDescending(x => x.CreatedAt)
                .Take(maxRecords)
                .ToArrayAsync();
        }

        return await context
            .Operations
            .AsNoTracking()
            .Where(x =>
                x.AccountId == portfolioIdentity.AccountId &&
                x.BookId == portfolioIdentity.BookId)
            .OrderByDescending(x => x.CreatedAt)
            .Take(maxRecords)
            .ToArrayAsync();
    }

    public async Task<bool> Save(TradeOperation[] operationEvents)
    {
        using var context = await GetDbContext();
        await context.Operations.AddRangeAsync(operationEvents);
        var savedRecords = await context.SaveChangesAsync();

        return savedRecords > 0;
    }

    public async Task<int> DeleteAll(PortfolioIdentity portfolioIdentity)
    {
        using var context = await GetDbContext();

        if (!portfolioIdentity.BookId.HasValue)
        {
            return await context.Operations
                .Where(s => s.AccountId == portfolioIdentity.AccountId)
                .ExecuteDeleteAsync();
        }

        return await context.Operations
            .Where(s => s.BookId == portfolioIdentity.BookId.Value)
            .ExecuteDeleteAsync();
    }
}
