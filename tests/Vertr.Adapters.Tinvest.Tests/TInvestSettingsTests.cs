using System.Text.Json;

namespace Vertr.Adapters.Tinvest.Tests;


public class TInvestSettingsTests
{
    [Test]
    public void CanSerializeSettingsToJson()
    {
        var settings = new TinvestSettings()
        {
            AccountId = "abcd",
            InvestApiSettings = new Tinkoff.InvestApi.InvestApiSettings()
            {
                AccessToken = "access token",
                AppName = "Test",
                Sandbox = true,
            },
            SymbolMappings = new Dictionary<string, string>()
            {
                {"SBER", "123123"},
                {"VTB", "13123"},
            },
        };

        var json = JsonSerializer.Serialize(settings);
        Console.WriteLine(json);
    }

    [Test]
    public void CanLoadSymbolMappings()
    {
    }
}
