using System.Text.Json;

namespace Vertr.Adapters.Tinvest.Tests;


public class TInvestSettingsTests : TinvestTestBase
{
    [Test]
    public void CanSerializeSettingsToJson()
    {
        var settings = new TinvestSettings()
        {
            Accounts = ["abcd", "12345"],
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
    public void CanGetSettings()
    {
        var settings = Settings;

        Assert.That(settings, Is.Not.Null);
        Assert.That(settings.SymbolMappings, Has.Count.GreaterThan(0));
    }

    [Test]
    public void CanGetSymbolIdFromSettings()
    {
        var symbolId = Settings.GetSymbolId("SBER");
        Assert.That(symbolId, Is.EqualTo("e6123145-9665-43e0-8413-cd61b8aa9b13"));
    }

    [Test]
    public void CanGetSymbolIdFromInvalidSymbolReturnsNull()
    {
        var symbolId = Settings.GetSymbolId("SSEE");
        Assert.That(symbolId, Is.Null);
    }

    [Test]
    public void CanGetAccounts()
    {
        var accounts = Settings.Accounts;
        Assert.That(accounts, Is.Not.Empty);

        foreach (var account in accounts)
        {
            Console.WriteLine(account);
        }
    }
}
