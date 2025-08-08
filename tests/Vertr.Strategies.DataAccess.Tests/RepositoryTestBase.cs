using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Vertr.Strategies.Contracts.Interfaces;

namespace Vertr.Strategies.DataAccess.Tests;

public abstract class RepositoryTestBase
{
    private readonly IServiceProvider _serviceProvider;

    protected IDbContextFactory<StrategiesDbContext> DbContextFactory => _serviceProvider.GetRequiredService<IDbContextFactory<StrategiesDbContext>>();

    protected IStrategyMetadataRepository StrategyRepo => _serviceProvider.GetRequiredService<IStrategyMetadataRepository>();

    protected RepositoryTestBase()
    {
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddStrategiesDataAccess(ConnectionStrings.LocalConnection);
        _serviceProvider = serviceCollection.BuildServiceProvider();
    }
}
