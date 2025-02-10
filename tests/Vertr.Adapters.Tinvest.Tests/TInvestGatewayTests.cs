using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Vertr.Domain.Ports;

namespace Vertr.Adapters.Tinvest.Tests;

public class TinvestGatewayTests : IDisposable
{
    private readonly ServiceProvider _serviceProvider;

    private readonly IConfiguration _configuration;

    protected ITinvestGateway Gateway => _serviceProvider.GetRequiredService<ITinvestGateway>();

    public TinvestGatewayTests()
    {
        _configuration = InitConfiguration();

        var services = new ServiceCollection();
        services.AddTinvestGateway(_configuration);

        _serviceProvider = services.BuildServiceProvider();
    }

    [Test]
    public async Task CanFindInstrument()
    {
        var instruments = await Gateway.FindInstrument("SBER");

        Assert.That(instruments, Is.Not.Null);

        foreach (var instrument in instruments)
        {
            Console.WriteLine($"{instrument}");
        }
    }

    public void Dispose()
    {
        _serviceProvider?.Dispose();
    }

    private static IConfiguration InitConfiguration()
    {
        var config = new ConfigurationBuilder()
           .AddJsonFile("appsettings.test.json")
           .AddEnvironmentVariables()
           .Build();

        return config;
    }
}
