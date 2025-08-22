using Vertr.Strategies.Contracts;

namespace Vertr.Strategies.DataAccess.Tests.Repositories;

[TestFixture(Category = "Database", Explicit = true)]
public class StrategyMetadataRepositoryTests : RepositoryTestBase
{
    [TestCase("e6123145-9665-43e0-8413-cd61b8aa9b13")]
    public async Task CanCreateStrategy(string instrumentId)
    {
        var meta = new StrategyMetadata
        {
            Id = Guid.NewGuid(),
            InstrumentId = Guid.Parse(instrumentId),
            Name = "Test Strategy",
            PortfolioId = Guid.Empty,
            IsActive = false,
            CreatedAt = DateTime.UtcNow,
            ParamsJson = null,
            Type = StrategyType.RandomWalk,
            QtyLots = 10,
        };

        var saved = await StrategyRepo.Save(meta);

        Assert.That(saved, Is.True);
    }

    [Test]
    public async Task CanGetStrategies()
    {
        var strategies = await StrategyRepo.GetAll();

        Assert.That(strategies, Is.Not.Null);

        foreach (var strategy in strategies)
        {
            Console.WriteLine(strategy);
        }
    }
}
