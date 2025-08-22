using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Vertr.PortfolioManager.Contracts.Interfaces;

namespace Vertr.PortfolioManager.DataAccess.Tests;

public abstract class RepositoryTestBase
{
    private readonly IServiceProvider _serviceProvider;

    protected IDbContextFactory<PortfolioDbContext> DbContextFactory => _serviceProvider.GetRequiredService<IDbContextFactory<PortfolioDbContext>>();

    protected IPortfolioRepository PortfolioRepo => _serviceProvider.GetRequiredService<IPortfolioRepository>();

    protected RepositoryTestBase()
    {
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddPortfolioManagerDataAccess(ConnectionStrings.LocalConnection);
        _serviceProvider = serviceCollection.BuildServiceProvider();
    }
}
