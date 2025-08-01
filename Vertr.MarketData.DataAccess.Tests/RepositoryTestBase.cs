using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Vertr.MarketData.Contracts.Interfaces;

namespace Vertr.MarketData.DataAccess.Tests;

public abstract class RepositoryTestBase
{
    private readonly IServiceProvider _serviceProvider;

    protected IDbContextFactory<MarketDataDbContext> DbContextFactory => _serviceProvider.GetRequiredService<IDbContextFactory<MarketDataDbContext>>();

    protected IMarketDataInstrumentRepository InstrumentsRepo => _serviceProvider.GetRequiredService<IMarketDataInstrumentRepository>();

    protected RepositoryTestBase()
    {
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddMarketDataAccess(ConnectionStrings.LocalConnection);
        _serviceProvider = serviceCollection.BuildServiceProvider();
    }
}
