using Microsoft.EntityFrameworkCore;

namespace Vertr.Adapters.DataAccess.Repositories;

internal abstract class RepositoryBase
{
    private readonly IDbContextFactory<VertrDbContext> _contextFactory;

    protected Task<VertrDbContext> GetDbContext() => _contextFactory.CreateDbContextAsync();

    protected RepositoryBase(IDbContextFactory<VertrDbContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }
}

