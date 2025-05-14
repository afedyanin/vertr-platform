using Microsoft.EntityFrameworkCore;

namespace Vertr.PortfolioManager.DataAccess.Repositories;

internal abstract class RepositoryBase
{
    private readonly IDbContextFactory<PortfolioDbContext> _contextFactory;

    protected Task<PortfolioDbContext> GetDbContext() => _contextFactory.CreateDbContextAsync();

    protected RepositoryBase(IDbContextFactory<PortfolioDbContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }
}

