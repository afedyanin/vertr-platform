using AutoMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Vertr.Domain.Ports;

namespace Vertr.Adapters.Tinvest.Tests;

public abstract class TinvestTestBase
{
    private readonly ServiceProvider _serviceProvider;
    private readonly IConfiguration _configuration;

    protected ITinvestGateway Gateway => _serviceProvider.GetRequiredService<ITinvestGateway>();

    protected IMapper TinvestMapper { get; private set; }

    protected TinvestSettings Settings { get; private set; }

    protected TinvestTestBase()
    {
        _configuration = ConfigFactory.GetConfiguration();

        var services = new ServiceCollection();
        services.AddTinvestGateway(_configuration);
        _serviceProvider = services.BuildServiceProvider();

        var options = _serviceProvider.GetRequiredService<IOptions<TinvestSettings>>();
        Settings = options.Value;

        TinvestMapper = _serviceProvider.GetRequiredService<IMapper>();
    }

    [OneTimeTearDown]
    public void TearDown()
    {
        _serviceProvider.Dispose();
    }
}
