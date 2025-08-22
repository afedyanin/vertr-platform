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
    public async Task CanGetPortfolios()
    {
        var portfolios = await PortfolioRepo.GetAll();

        Assert.That(portfolios, Is.Not.Null);

        foreach (var portfolio in portfolios)
        {
            Console.WriteLine($"Id={portfolio.Id} Name={portfolio.Name}");

            foreach (var position in portfolio.Positions)
            {
                Console.WriteLine($"positionId={position.Id} balance={position.Balance}");
            }
        }
    }
}
