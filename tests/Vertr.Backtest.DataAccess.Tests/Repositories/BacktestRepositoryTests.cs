using Vertr.Backtest.Contracts;

namespace Vertr.Backtest.DataAccess.Tests.Repositories;

[TestFixture(Category = "Database", Explicit = true)]
public class BacktestRepositoryTests : RepositoryTestBase
{
    [Test]
    public async Task CanCreateBacktest()
    {
        var backtest = new BacktestRun
        {
            Id = Guid.NewGuid(),
            From = new DateTime(2025, 8, 1, 0, 0, 0, DateTimeKind.Utc),
            To = new DateTime(2025, 8, 11, 0, 0, 0, DateTimeKind.Utc),
            Description = "Test backtest",
            StrategyId = Guid.NewGuid(),
            SubAccountId = Guid.Empty,
            ExecutionState = ExecutionState.Created,
            CreatedAt = DateTime.UtcNow,
            ProgressMessage = "Backtest Created",
            IsCancellationRequested = false,
        };

        var saved = await BacktestRepo.Save(backtest);

        Assert.That(saved, Is.True);
    }

    [Test]
    public async Task CanGetStrategies()
    {
        var backtests = await BacktestRepo.GetAll();

        Assert.That(backtests, Is.Not.Null);

        foreach (var backtest in backtests)
        {
            Console.WriteLine(backtest);
        }
    }
}
