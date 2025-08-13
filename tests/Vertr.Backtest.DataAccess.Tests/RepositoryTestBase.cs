using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Vertr.Backtest.Contracts.Interfaces;
using Vertr.Strategies.DataAccess.Tests;

namespace Vertr.Backtest.DataAccess.Tests;

public abstract class RepositoryTestBase
{
    private readonly IServiceProvider _serviceProvider;

    protected IDbContextFactory<BacktestDbContext> DbContextFactory => _serviceProvider.GetRequiredService<IDbContextFactory<BacktestDbContext>>();

    protected IBacktestRepository BacktestRepo => _serviceProvider.GetRequiredService<IBacktestRepository>();

    protected RepositoryTestBase()
    {
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddBacktestDataAccess(ConnectionStrings.LocalConnection);
        _serviceProvider = serviceCollection.BuildServiceProvider();
    }
}
