using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Vertr.Clients.MoexApiClient;

namespace Vetr.Moex.ApiClient.Tests;

public class MoexApiClientTests
{
#pragma warning disable NUnit1032 // An IDisposable field/property should be Disposed in a TearDown method
    private readonly IServiceProvider _serviceProvider;
#pragma warning restore NUnit1032 // An IDisposable field/property should be Disposed in a TearDown method
    private readonly IMoexApiClient _moexApiClient;

    public MoexApiClientTests()
    {
        var services = new ServiceCollection();
        services.AddHttpClient();
        services.AddMoexApiClient();

        _serviceProvider = services.BuildServiceProvider();
        _moexApiClient = _serviceProvider.GetRequiredService<IMoexApiClient>();
    }

    [Test]
    public async Task CanGetFutureStaticData()
    {
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        using var httpClient = new HttpClient();
        var csv = await httpClient.GetStringAsync("https://iss.moex.com/iss/securities/SRH6.csv?iss.meta=on&iss.only=description");
        Console.WriteLine(csv);
    }

    [TestCase("SRH6")]
    [TestCase("SBERF")]
    public async Task CanGetSecurityInfoItems(string ticker)
    {
        var items = await _moexApiClient.GetSecurityInfo(ticker);

        foreach (var item in items)
        {
            Console.WriteLine(item);
        }
    }

    [Test]
    public async Task CanGetFutureInfo()
    {
        var futureInfos = await _moexApiClient.GetFutureInfo("SRH6", "SRU6", "SRM6", "SBERF");

        foreach (var futureInfo in futureInfos)
        {
            Console.WriteLine(futureInfo);
        }
    }

    [TestCase("RUSFAR")]
    public async Task CanGetIndexCandles(string ticker)
    {
        var items = await _moexApiClient.GetIndexCandles(ticker);

        foreach (var item in items)
        {
            Console.WriteLine(item);
        }
    }

    [TestCase("RUSFAR")]
    public async Task CanGetIndexRates(string ticker)
    {
        var from = new DateOnly(2025, 12, 10);
        var items = await _moexApiClient.GetIndexRates(ticker, from);

        foreach (var item in items)
        {
            Console.WriteLine(item);
        }
    }
}
