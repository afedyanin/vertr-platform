using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Vertr.Domain.Ports;

namespace Vertr.Adapters.Tinvest.Tests;

public class TinvestGatewayTests : IDisposable
{
    private const string _sber_uid = "e6123145-9665-43e0-8413-cd61b8aa9b13";

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

    [Test]
    public async Task CanGetInstrument()
    {
        var instrument = await Gateway.GetInstrument("SBER", "TQBR");

        Assert.That(instrument, Is.Not.Null);

        Console.WriteLine($"{instrument}");
    }

    [Test]
    public async Task CanGetCandles()
    {
        var to = DateTime.UtcNow;
        var from = to.AddDays(-1);

        var candles = await Gateway.GetCandles(
            _sber_uid,
            Domain.CandleInterval.Min10,
            from,
            to);

        Assert.That(candles, Is.Not.Null);
        Assert.That(candles.Count, Is.GreaterThanOrEqualTo(1));

        foreach (var candle in candles)
        {
            Console.WriteLine($"{candle}");
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
