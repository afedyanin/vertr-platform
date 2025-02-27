using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Vertr.Domain;
using Vertr.Domain.Repositories;

namespace Vertr.Adapters.DataAccess.Tests.Repositories;

[TestFixture(Category = "database", Explicit = true)]
public class TinvestPortfolioRepositoryTests
{
    private readonly ServiceProvider _serviceProvider;

    private readonly IConfiguration _configuration;

    protected ITinvestPortfolioRepository Repo => _serviceProvider.GetRequiredService<ITinvestPortfolioRepository>();

    public TinvestPortfolioRepositoryTests()
    {
        _configuration = ConfigFactory.InitConfiguration();

        var services = new ServiceCollection();
        services.AddDataAccess(_configuration);

        _serviceProvider = services.BuildServiceProvider();
    }

    [OneTimeTearDown]
    public void TearDown()
    {
        _serviceProvider.Dispose();
    }

    [Test]
    public async Task CanInsertPortfolioSnapshot()
    {
        var snapshot = new PortfolioSnapshot
        {
            Id = Guid.NewGuid(),
            AccountId = "12345",
            ExpectedYield = 0.1213m,
            TimeUtc = DateTime.UtcNow,
            TotalAmountBonds = 123.45m,
            TotalAmountCurrencies = 123.45m,
            TotalAmountEtf = 123.45m,
            TotalAmountFutures = 123.45m,
            TotalAmountOptions = 123.45m,
            TotalAmountPortfolio = 123.45m,
            TotalAmountShares = 123.45m,
            TotalAmountSp = 123.45m,
        };

        var pos1 = new PortfolioPosition
        {
            Id = Guid.NewGuid(),
            PortfolioSnapshotId = snapshot.Id,
            AveragePositionPrice = 123.45m,
            AveragePositionPriceFifo = 34.23m,
            Blocked = false,
            BlockedLots = 0,
            CurrentNkd = 0,
            CurrentPrice = 0,
            ExpectedYield = 0,
            ExpectedYieldFifo = 0,
            InstrumentType = "shares",
            InstrumentUid = Guid.NewGuid(),
            PortfolioSnapshot = snapshot,
            PositionUid = Guid.NewGuid(),
            Quantity = 1923,
            VarMargin = 0,
        };

        var pos2 = new PortfolioPosition
        {
            Id = Guid.NewGuid(),
            PortfolioSnapshotId = snapshot.Id,
            AveragePositionPrice = 123.45m,
            AveragePositionPriceFifo = 34.23m,
            Blocked = false,
            BlockedLots = 0,
            CurrentNkd = 0,
            CurrentPrice = 0,
            ExpectedYield = 0,
            ExpectedYieldFifo = 0,
            InstrumentType = "currencies",
            InstrumentUid = Guid.NewGuid(),
            PortfolioSnapshot = snapshot,
            PositionUid = Guid.NewGuid(),
            Quantity = 1923,
            VarMargin = 0,
        };

        snapshot.Positions = [pos1, pos2];
        var rows = await Repo.Insert(snapshot);
        Assert.That(rows, Is.GreaterThan(0));
    }

    [Test]
    public async Task CanGetPortfolioSnapshot()
    {
        var accountId = "12345";

        var result = await Repo.Get(accountId);

        var snapshot = result.First();

        Assert.That(snapshot.Positions.Any(), Is.False);

        Console.WriteLine(snapshot);
    }

    [Test]
    public async Task CanGetPortfolioSnapshotById()
    {
        var id = new Guid("4f2936b3-70c1-43e9-be29-431444dc3001");

        var snapshot = await Repo.GetById(id);

        Assert.That(snapshot, Is.Not.Null);
        Assert.That(snapshot.Positions.Any(), Is.True);

        Console.WriteLine(snapshot);

        foreach (var position in snapshot.Positions)
        {
            Console.WriteLine($"-->{position}");
        }
    }
}
