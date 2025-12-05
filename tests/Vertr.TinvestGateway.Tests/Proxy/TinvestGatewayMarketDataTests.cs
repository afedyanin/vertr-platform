using System.Net.Http.Headers;
using Tinkoff.InvestApi;
using Vertr.TinvestGateway.Proxy;

namespace Vertr.TinvestGateway.Tests.Proxy;

[TestFixture(Category = "Gateway", Explicit = true)]
public class TinvestGatewayMarketDataTests
{
    private InvestApiClient _client;

    [OneTimeSetUp]
    public void OneTimeSetup()
    {
        _client = InvestApiClientFactory.Create(Credentials.ApiSettings);
    }

    [TestCase("SBER")]
    [TestCase("RUB")]
    [TestCase("RUB000UTSTOM")]
    public async Task CanFindInstrument(string query)
    {
        var gateway = new TinvestGatewayMarketData(_client);

        var found = await gateway.FindInstrument(query);

        foreach (var instrument in found!)
        {
            Console.WriteLine(instrument);
        }

        Assert.Pass();
    }

    [TestCase("a92e2e25-a698-45cc-a781-167cf465257c")] // RUB
    public async Task CanGetInstrumentById(string instrumentId)
    {
        var gateway = new TinvestGatewayMarketData(_client);
        var instrument = await gateway.GetInstrumentById(Guid.Parse(instrumentId));

        Console.WriteLine(instrument);

        Assert.Pass();
    }

    [TestCase("TQBR", "SBER")]
    public async Task CanGetInstrumentBySymbol(string classCode, string ticker)
    {
        var gateway = new TinvestGatewayMarketData(_client);
        var instrument = await gateway.GetInstrumentBySymbol(classCode, ticker);

        Console.WriteLine(instrument);

        Assert.Pass();
    }

    [TestCase("e6123145-9665-43e0-8413-cd61b8aa9b13")] // SBER
    public async Task CanGetCandles(string instrumentId)
    {
        var gateway = new TinvestGatewayMarketData(_client);
        //var day = new DateOnly(2025, 07, 30);
        var day = DateOnly.FromDateTime(DateTime.UtcNow);

        var candles = await gateway.GetCandles(Guid.Parse(instrumentId), day);

        Assert.That(candles, Is.Not.Null);

        var first = candles.First();
        var last = candles.Last();

        Console.WriteLine($"Count={candles.Length} From={first.TimeUtc:O} To={last.TimeUtc:O}");
        Assert.Pass();
    }



    [TestCase("BBG004730N88", 2025, "sber_2025.zip")] // SBER
    public async Task CanLoadAllHistory(string figi, int year, string filePath)
    {
        var httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Credentials.ApiSettings.AccessToken);
        httpClient.BaseAddress = new Uri("https://invest-public-api.tinkoff.ru");

        var response = await httpClient.GetAsync($"/history-data?figi={figi}&year={year}");

        if (!response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            Console.WriteLine(content);
        }

        response.EnsureSuccessStatusCode();

        using var fileStream = new FileStream(filePath, FileMode.Create);
        using var stream = await response.Content.ReadAsStreamAsync();

        await stream.CopyToAsync(fileStream);
    }


    [TestCase("e6123145-9665-43e0-8413-cd61b8aa9b13")] // SBER
    public async Task CanGetCandlesForAllDays(string instrumentId)
    {
        var gateway = new TinvestGatewayMarketData(_client);
        var day = new DateOnly(2025, 08, 14);

        for (var i = 0; i <= 500; i++)
        {
            var candles = await gateway.GetCandles(Guid.Parse(instrumentId), day);
            day = day.AddDays(-1);
            Console.WriteLine($"Day={day:O} Candes count={candles?.Length}");
        }

        Assert.Pass();
    }
}