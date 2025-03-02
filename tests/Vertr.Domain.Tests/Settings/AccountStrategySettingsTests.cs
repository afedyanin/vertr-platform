using System.Text.Json;
using Vertr.Domain.Settings;
using Microsoft.Extensions.Configuration;
using Vertr.Domain.Enums;

namespace Vertr.Domain.Tests.Settings;

[TestFixture(Category = "Integration", Explicit = true)]
public class AccountStrategySettingsTests
{
    [Test]
    public void CanSerializeSettings()
    {
        var s1 = new StrategySettings
        {
            Symbol = "SBER",
            PredictorType = PredictorType.RandomWalk,
            Sb3Algo = Sb3Algo.Undefined,
        };

        var s2 = new StrategySettings
        {
            Symbol = "VTBR",
            PredictorType = PredictorType.Sb3,
            Sb3Algo = Sb3Algo.SAC,
        };

        var settings = new AccountStrategySettings
        {
            SignalMappings = new Dictionary<string, IList<StrategySettings>>
            {
                { "1234", [s1, s2] }
            }
        };

        var json = JsonSerializer.Serialize(settings);
        Assert.That(json, Is.Not.Null);

        Console.WriteLine(json);
    }

    [Test]
    public void CanLoadSettingsFromConfig()
    {
        var config = ConfigFactory.GetConfiguration();

        var settings = new AccountStrategySettings();

        config.GetSection(nameof(AccountStrategySettings)).Bind(settings);

        var mapping = settings.SignalMappings;

        Assert.That(mapping.Keys, Has.Count.GreaterThan(0));

        foreach (var key in mapping.Keys)
        {
            var items = mapping[key];
            var values = string.Join(";", items);
            Console.WriteLine($"key={key} values={values}");
        }
    }
}
