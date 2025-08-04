using Vertr.MarketData.Contracts;

namespace Vertr.MarketData.DataAccess.Tests.Repositories;

[TestFixture(Category = "Database", Explicit = true)]
public class SubscriptionsRepositoryTests : RepositoryTestBase
{
    [TestCase("e6123145-9665-43e0-8413-cd61b8aa9b13")]
    public async Task CanCreateSubscription(string instrumentId)
    {
        var subscription = new CandleSubscription
        {
            Id = Guid.NewGuid(),
            InstrumentId = Guid.Parse(instrumentId),
            Interval = CandleInterval.Min_1,
            Disabled = false,
            LoadHistory = true,
            ExternalStatus = null,
            ExternalSubscriptionId = null,
        };

        var saved = await SubscriptionsRepo.Save(subscription);

        Assert.That(saved, Is.True);
    }

    [Test]
    public async Task CanGetSubscriptions()
    {
        var susbs = await SubscriptionsRepo.GetAll();

        Assert.That(susbs, Is.Not.Null);

        foreach (var susb in susbs)
        {
            Console.WriteLine(susb);
        }
    }
}
