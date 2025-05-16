using Microsoft.EntityFrameworkCore;

namespace Vertr.OrderExecution.DataAccess.Repositories;

internal abstract class RepositoryBase
{
    private readonly IDbContextFactory<OrderExecutionDbContext> _contextFactory;

    protected Task<OrderExecutionDbContext> GetDbContext() => _contextFactory.CreateDbContextAsync();

    protected RepositoryBase(IDbContextFactory<OrderExecutionDbContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }
}

