using Microsoft.EntityFrameworkCore;
using Vertr.PortfolioManager.Contracts;
using Vertr.PortfolioManager.Contracts.Interfaces;

namespace Vertr.PortfolioManager.DataAccess.Repositories;
internal class PortfolioRepository : RepositoryBase, IPortfolioRepository
{
    public PortfolioRepository(IDbContextFactory<PortfolioDbContext> contextFactory) : base(contextFactory)
    {
    }

    public async Task<Portfolio[]> GetAll()
    {
        using var context = await GetDbContext();

        return await context
            .Portfolios
            .Include(p => p.Positions)
            .OrderByDescending(x => x.UpdatedAt)
            .ToArrayAsync();
    }

    public async Task<Portfolio?> GetById(Guid portfolioId)
    {
        using var context = await GetDbContext();

        return context
            .Portfolios
            .Include(p => p.Positions)
            .SingleOrDefault(x => x.Id == portfolioId);
    }

    public async Task<bool> Save(Portfolio portfolio)
    {
        using var context = await GetDbContext();

        var existing = context
            .Portfolios
            .Include(p => p.Positions)
            .Any(x => x.Id == portfolio.Id);

        if (existing)
        {
            context.Attach(portfolio);
        }
        else
        {
            context.Portfolios.Add(portfolio);
        }

        var savedRecords = await context.SaveChangesAsync();

        return savedRecords > 0;
    }

    public async Task<int> Delete(Guid portfolioId)
    {
        using var context = await GetDbContext();

        return await context.Portfolios
            .Where(s => s.Id == portfolioId)
            .ExecuteDeleteAsync();
    }
}
