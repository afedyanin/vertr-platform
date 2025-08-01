using Tinkoff.InvestApi;
using Vertr.MarketData.Contracts;
using Vertr.TinvestGateway.Application.Proxy;

namespace Vertr.TinvestGateway.Application.Tests;

[TestFixture(Category = "System", Explicit = true)]
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
}
