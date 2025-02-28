using Vertr.Domain.Repositories;
using Vertr.Application.Portfolios;
using Microsoft.Extensions.DependencyInjection;

namespace Vertr.Application.Tests.Portfolios;

[TestFixture(Category = "System", Explicit = true)]
public class LoadPortfolioSnapshotsHandlerTests : ApplicationTestBase
{

    [Test]
    public async Task CanLoadAndSavePortfolios()
    {
        var request = new LoadPortfolioSnapshotsRequest();
        var handler = ServiceProvider.GetRequiredService<LoadPortfolioSnapshotsHandler>();

        await handler.Handle(request, CancellationToken.None);

        var repo = ServiceProvider.GetRequiredService<ITinvestPortfolioRepository>();

        var snapshots = repo.Get();

        Assert.Pass();
    }
}
