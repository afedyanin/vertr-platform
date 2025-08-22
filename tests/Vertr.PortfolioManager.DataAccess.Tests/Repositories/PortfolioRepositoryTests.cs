using Vertr.PortfolioManager.Contracts;

namespace Vertr.PortfolioManager.DataAccess.Tests.Repositories;

[TestFixture(Category = "Database", Explicit = true)]
public class PortfolioRepositoryTests : RepositoryTestBase
{
    [Test]
    public async Task CanCreatePortfolio()
    {
        var portfolio = new Portfolio
        {
            Id = Guid.NewGuid(),
            Name = "Test Portfolio",
        };

        var saved = await PortfolioRepo.Save(portfolio);

        Assert.That(saved, Is.True);
    }

    [Test]
    public async Task CanCreatePortfolioWithPositions()
    {
        var portfolio = new Portfolio
        {
            Id = Guid.NewGuid(),
            Name = "Test Portfolio",
        };

        var pos = new Position
        {
            Id = Guid.NewGuid(),
            PortfolioId = portfolio.Id,
            InstrumentId = Guid.Empty,
            Balance = 35.47m
        };

        portfolio.Positions.Add(pos);
        var saved = await PortfolioRepo.Save(portfolio);

        Assert.That(saved, Is.True);
    }

    [Test]
    public async Task CanUpdatePortfolio()
    {
        var portfolio = new Portfolio
        {
            Id = Guid.NewGuid(),
            Name = "Test Portfolio with positions",
        };

        var saved = await PortfolioRepo.Save(portfolio);
        Assert.That(saved, Is.True);

        var found = await PortfolioRepo.GetById(portfolio.Id);
        Assert.That(found, Is.Not.Null);

        var pos = new Position
        {
            Id = Guid.NewGuid(),
            PortfolioId = portfolio.Id,
            InstrumentId = Guid.Empty,
            Balance = 35.47m
        };

        found.Positions.Add(pos);

        saved = await PortfolioRepo.Save(found);
        Assert.That(saved, Is.True);
    }

    [Test]
    public async Task CanUpdatePortfolioWithPositions()
    {
        // step 1. Create portfolio with positions
        var portfolio = new Portfolio
        {
            Id = Guid.NewGuid(),
            Name = "Test Portfolio with positions",
        };

        var pos1 = new Position
        {
            Id = Guid.NewGuid(),
            PortfolioId = portfolio.Id,
            InstrumentId = Guid.NewGuid(),
            Balance = 100
        };
        portfolio.Positions.Add(pos1);

        var pos2 = new Position
        {
            Id = Guid.NewGuid(),
            PortfolioId = portfolio.Id,
            InstrumentId = Guid.NewGuid(),
            Balance = 200
        };
        portfolio.Positions.Add(pos2);

        var pos3 = new Position
        {
            Id = Guid.NewGuid(),
            PortfolioId = portfolio.Id,
            InstrumentId = Guid.NewGuid(),
            Balance = 300
        };
        portfolio.Positions.Add(pos3);

        var saved = await PortfolioRepo.Save(portfolio);
        Assert.That(saved, Is.True);

        var found = await PortfolioRepo.GetById(portfolio.Id);
        Assert.That(found, Is.Not.Null);
        Assert.That(found.Positions, Has.Count.EqualTo(portfolio.Positions.Count));
        DumpPortfolio(found);

        // step 2. Add new position (400)
        var posAdded = new Position
        {
            Id = Guid.NewGuid(),
            PortfolioId = portfolio.Id,
            InstrumentId = Guid.Empty,
            Balance = 400
        };

        found.Positions.Add(posAdded);

        // step 3. Remove position (300)
        found.Positions.Remove(found.Positions.Single(p => p.Id == pos3.Id));

        // step 4. Update position (200 -> 250)
        found.Positions.Single(p => p.Id == pos2.Id).Balance = 250;

        saved = await PortfolioRepo.Save(found);
        Assert.That(saved, Is.True);

        var result = await PortfolioRepo.GetById(found.Id);
        Assert.That(result, Is.Not.Null);
        DumpPortfolio(result);
    }

    [Test]
    public async Task CanGetPortfolios()
    {
        var portfolios = await PortfolioRepo.GetAll();

        Assert.That(portfolios, Is.Not.Null);

        foreach (var portfolio in portfolios)
        {
            DumpPortfolio(portfolio);
        }
    }

    private void DumpPortfolio(Portfolio portfolio)
    {
        Console.WriteLine($"Id={portfolio.Id} Name={portfolio.Name}");

        foreach (var position in portfolio.Positions)
        {
            Console.WriteLine($"-- positionId={position.Id} balance={position.Balance}");
        }

        Console.WriteLine();
    }
}
