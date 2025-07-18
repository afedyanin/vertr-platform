using Vertr.MarketData.Application.Repositories;

namespace Vertr.MarketData.Application.Tests;

public class MarketDataRepositoryTests
{
    [Test]
    public void CanCreateRepository()
    {
        var repo = new MarketDataRepository();
        Assert.That(repo, Is.Not.Null);
    }
}
