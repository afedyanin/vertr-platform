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

    public async Task<TradeOperation[]> GetAll()
    {
        using var context = await GetDbContext();

        return await context
            .Operations
            .OrderBy(x => x.CreatedAt)
            .ToArrayAsync();
    }

    public async Task<TradeOperation[]> GetByPortfolio(PortfolioIdentity portfolioIdentity)
    {
        using var context = await GetDbContext();

        return await context
            .Operations
            .Where(x =>
                x.AccountId == portfolioIdentity.AccountId &&
                x.SubAccountId == portfolioIdentity.SubAccountId)
            .OrderBy(x => x.CreatedAt)
            .ToArrayAsync();
    }

    public async Task<TradeOperation[]> GetByAccountId(string accountId)
    {
        using var context = await GetDbContext();

        return await context
            .Operations
            .Where(x => x.AccountId == accountId)
            .OrderBy(x => x.CreatedAt)
            .ToArrayAsync();
    }

    public async Task<bool> Save(TradeOperation[] operationEvents)
    {
        using var context = await GetDbContext();
        await context.Operations.AddRangeAsync(operationEvents);
        var savedRecords = await context.SaveChangesAsync();

        return savedRecords > 0;
    }

    public async Task<int> Delete(PortfolioIdentity portfolioIdentity)
    {
        using var context = await GetDbContext();

        return await context.Operations
            .Where(s =>
            s.AccountId == portfolioIdentity.AccountId &&
            s.SubAccountId == portfolioIdentity.SubAccountId)
            .ExecuteDeleteAsync();
    }

    public async Task<int> DeleteByAccountId(string accountId)
    {
        using var context = await GetDbContext();

        return await context.Operations
            .Where(s => s.AccountId == accountId)
            .ExecuteDeleteAsync();
    }
}
