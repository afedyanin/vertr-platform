using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Vertr.CommandLine.Application;
using Vertr.CommandLine.Common.Mediator;
using Vertr.CommandLine.Models.Abstracttions;
using Vertr.CommandLine.Predictor.Client;

namespace Vertr.CommandLine.Models.Tests;

public abstract class SystemTestBase
{
    private const string BaseAddress = "http://127.0.0.1:8081";

    private readonly IServiceProvider _serviceProvider;
    private readonly ILoggerFactory _loggerFactory;

    protected IMediator Mediator => _serviceProvider.GetRequiredService<IMediator>();
    protected IMarketDataService MarketDataService => _serviceProvider.GetRequiredService<IMarketDataService>();

    protected IPortfolioService PortfolioService => _serviceProvider.GetRequiredService<IPortfolioService>();

    //protected ILogger Logger = NullLoggerFactory.Instance.CreateLogger<SystemTestBase>();
    protected ILogger Logger => _loggerFactory.CreateLogger<SystemTestBase>();

    protected SystemTestBase()
    {
        var services = new ServiceCollection();

        services.AddMediator();
        services.AddApplication();
        services.AddPredictionService(BaseAddress);

        _loggerFactory = LoggerFactory.Create(builder =>
        {
            builder.AddConsole();
            builder.SetMinimumLevel(LogLevel.Information); // Set minimum log level
        });

        _serviceProvider = services.BuildServiceProvider();
    }
}