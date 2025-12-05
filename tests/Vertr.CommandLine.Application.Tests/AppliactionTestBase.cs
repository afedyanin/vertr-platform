using Microsoft.Extensions.DependencyInjection;
using Vertr.CommandLine.Common.Mediator;
using Vertr.CommandLine.Models.Abstracttions;

namespace Vertr.CommandLine.Application.Tests;

public abstract class AppliactionTestBase
{
    private readonly IServiceProvider _serviceProvider;

    protected IMediator Mediator => _serviceProvider.GetRequiredService<IMediator>();
    protected IMarketDataService MarketDataService => _serviceProvider.GetRequiredService<IMarketDataService>();

    protected AppliactionTestBase()
    {
        IServiceCollection services = new ServiceCollection();

        services.AddMediator();
        services.AddApplication();

        _serviceProvider = services.BuildServiceProvider();
    }
}