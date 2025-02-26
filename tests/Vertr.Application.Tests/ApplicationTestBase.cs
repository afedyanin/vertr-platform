using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Vertr.Adapters.Tinvest;
using Vertr.Domain.Ports;
using Vertr.Adapters.DataAccess;
using Microsoft.Extensions.Logging;
using Vertr.Application.Candles;
using Vertr.Adapters.Prediction;
using Vertr.Application.Signals;
using Vertr.Domain.Repositories;

namespace Vertr.Application.Tests;

public abstract class ApplicationTestBase
{
    private readonly IConfiguration _configuration;

    protected ITinvestGateway Gateway => ServiceProvider.GetRequiredService<ITinvestGateway>();
    protected ITinvestCandlesRepository Repo => ServiceProvider.GetRequiredService<ITinvestCandlesRepository>();
    protected IOptions<TinvestSettings> Settings { get; private set; }
    protected ServiceProvider ServiceProvider { get; private set; }

    protected ApplicationTestBase()
    {
        _configuration = ConfigFactory.GetConfiguration();

        var services = new ServiceCollection();

        services.AddTinvestGateway(_configuration);
        services.AddDataAccess(_configuration);
        services.AddApplication();

        services.AddPredictions(httpClient =>
            httpClient.BaseAddress = new Uri("http://127.0.0.1:8081"));

        services.AddLogging(builder => builder
            .AddConsole()
            .AddConfiguration(_configuration.GetSection("Logging")));

        services.AddTransient<UpdateLastCandlesHandler>();
        services.AddTransient<GenerateSignalsHandler>();

        ServiceProvider = services.BuildServiceProvider();

        var options = ServiceProvider.GetRequiredService<IOptions<TinvestSettings>>();
        Settings = options;
    }

    [OneTimeTearDown]
    public void TearDown()
    {
        ServiceProvider.Dispose();
    }

}
