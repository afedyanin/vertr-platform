using Vertr.MarketData.Application.Repositories;

namespace Vertr.MarketData.Application.Tests;

public class CandleRepositoryTests
{
    [Test]
    public void CanCreateRepo()
    {
        var repo = new CandleRepository();
        Assert.That(repo, Is.Not.Null);
    }
}
