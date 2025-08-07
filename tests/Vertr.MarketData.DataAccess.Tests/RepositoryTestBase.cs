using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Vertr.MarketData.Contracts.Interfaces;

namespace Vertr.MarketData.DataAccess.Tests;

public abstract class RepositoryTestBase
{
    private readonly IServiceProvider _serviceProvider;

    protected IDbContextFactory<MarketDataDbContext> DbContextFactory => _serviceProvider.GetRequiredService<IDbContextFactory<MarketDataDbContext>>();

    protected IInstrumentsRepository InstrumentsRepo => _serviceProvider.GetRequiredService<IInstrumentsRepository>();
    protected ICandlesRepository CandlesRepo => _serviceProvider.GetRequiredService<ICandlesRepository>();
    protected ICandlesHistoryRepository CandlesHistoryRepo => _serviceProvider.GetRequiredService<ICandlesHistoryRepository>();
    protected ISubscriptionsRepository SubscriptionsRepo => _serviceProvider.GetRequiredService<ISubscriptionsRepository>();

    protected RepositoryTestBase()
    {
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddMarketDataAccess(ConnectionStrings.LocalConnection);
        _serviceProvider = serviceCollection.BuildServiceProvider();
    }
}
