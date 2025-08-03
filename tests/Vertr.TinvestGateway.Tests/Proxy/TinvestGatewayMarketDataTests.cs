using Tinkoff.InvestApi;
using Vertr.MarketData.Contracts;
using Vertr.TinvestGateway.Proxy;

namespace Vertr.TinvestGateway.Tests;

[TestFixture(Category = "Gateway", Explicit = true)]
public class TinvestGatewayMarketDataTests
{
    private static readonly InvestApiSettings _settings = new InvestApiSettings()
    {
        AccessToken = "t.8DpIsag8_t2bHcaPEXZiAxDLdxbyqP7MXvDwoamPBWSDBD7dgQeMNutgas5Ay83YOlLsA-m8qSPm8Sz-FMaNuw",
        AppName = "VERTR",
        Sandbox = true,
    };

    private InvestApiClient _client;

    [OneTimeSetUp]
    public void OneTimeSetup()
    {
        _client = InvestApiClientFactory.Create(_settings);
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

    [TestCase("a92e2e25-a698-45cc-a781-167cf465257c")]
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
        var symbol = new Symbol(classCode, ticker);
        var instrument = await gateway.GetInstrumentBySymbol(symbol);

        Console.WriteLine(instrument);

        Assert.Pass();
    }

    [TestCase("e6123145-9665-43e0-8413-cd61b8aa9b13")]
    public async Task CanGetCandles(string instrumentId)
    {
        var from = new DateTime(2025, 07, 29, 20, 0, 0);
        var to = new DateTime(2025, 07, 31);
        var gateway = new TinvestGatewayMarketData(_client);
        var candles = await gateway.GetCandles(Guid.Parse(instrumentId), CandleInterval.Min_1, from, to, 100);

        foreach (var candle in candles)
        {
            Console.WriteLine(candle);
        }

        Assert.Pass();
    }
}
