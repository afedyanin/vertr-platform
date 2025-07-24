using Microsoft.EntityFrameworkCore;

namespace Vertr.Strategies.DataAccess.Repositories;

internal abstract class RepositoryBase
{
    private readonly IDbContextFactory<StrategiesDbContext> _contextFactory;

    protected Task<StrategiesDbContext> GetDbContext() => _contextFactory.CreateDbContextAsync();

    protected RepositoryBase(IDbContextFactory<StrategiesDbContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }
}
