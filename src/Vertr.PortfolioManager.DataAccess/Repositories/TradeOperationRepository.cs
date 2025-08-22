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
            .OrderByDescending(x => x.CreatedAt)
            .ToArrayAsync();
    }

    public async Task<TradeOperation[]> GetByPortfolio(Guid portfolioId)
    {
        using var context = await GetDbContext();

        return await context
            .Operations
            .Where(x => x.PortfolioId == portfolioId)
            .OrderByDescending(x => x.CreatedAt)
            .ToArrayAsync();
    }

    public async Task<TradeOperation[]> GetByAccountId(string accountId)
    {
        using var context = await GetDbContext();

        return await context
            .Operations
            .Where(x => x.AccountId == accountId)
            .OrderByDescending(x => x.CreatedAt)
            .ToArrayAsync();
    }

    public async Task<bool> Save(TradeOperation operation)
    {
        using var context = await GetDbContext();
        context.Operations.Add(operation);
        var savedRecords = await context.SaveChangesAsync();

        return savedRecords > 0;
    }

    public async Task<int> Delete(Guid portfolioId)
    {
        using var context = await GetDbContext();

        return await context.Operations
            .Where(s => s.PortfolioId == portfolioId)
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
