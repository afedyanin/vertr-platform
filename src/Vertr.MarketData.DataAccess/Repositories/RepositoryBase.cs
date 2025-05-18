using Microsoft.EntityFrameworkCore;

namespace Vertr.MarketData.DataAccess.Repositories;

internal abstract class RepositoryBase
{
    private readonly IDbContextFactory<MarketDataDbContext> _contextFactory;

    protected Task<MarketDataDbContext> GetDbContext() => _contextFactory.CreateDbContextAsync();

    protected RepositoryBase(IDbContextFactory<MarketDataDbContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }
}

