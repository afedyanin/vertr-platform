using Microsoft.EntityFrameworkCore;

namespace Vertr.Backtest.DataAccess.Repositories;

internal abstract class RepositoryBase
{
    private readonly IDbContextFactory<BacktestDbContext> _contextFactory;

    protected Task<BacktestDbContext> GetDbContext() => _contextFactory.CreateDbContextAsync();

    protected RepositoryBase(IDbContextFactory<BacktestDbContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }
}
