using Vertr.TinvestGateway.Contracts.Portfolios;

namespace Vertr.TinvestGateway.Tests.PositionTests;

public class PositionComposerTests
{
    [Test]
    public async Task CanComposeComissions()
    {
        var items = CreateMoneyItems();

        var grouped = items.GroupBy(m => m.Currency).Select(g => new Position
        {
            InstrumentId = Guid.NewGuid(),
            Balance = g.Sum(o => o.Value)
        });

        foreach (var item in grouped)
        {
            Console.WriteLine(item);
        }
    }


    private static IEnumerable<Money> CreateMoneyItems()
        => [
            new Money(10, "rub"),
            new Money(12, "usd"),
            new Money(20, "rub"),
            new Money(12, "usd"),
            new Money(30, "rub"),
            new Money(12, "usd"),
            new Money(40, "rub"),
            new Money(12, "usd"),
            new Money(50, "rub"),
            new Money(60, "rub"),
            new Money(12, "eur"),
            new Money(70, "rub"),
            new Money(12, "eur"),
            new Money(10, "rub"),
            new Money(12, "usd"),
        ];
}