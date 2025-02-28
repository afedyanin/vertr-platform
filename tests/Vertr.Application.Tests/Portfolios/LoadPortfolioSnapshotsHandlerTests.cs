using Vertr.Domain.Repositories;
using Vertr.Application.Portfolios;
using Microsoft.Extensions.DependencyInjection;
using MediatR;

namespace Vertr.Application.Tests.Portfolios;

[TestFixture(Category = "System", Explicit = true)]
public class LoadPortfolioSnapshotsHandlerTests : ApplicationTestBase
{
    [Test]
    public async Task CanLoadAndSavePortfolios()
    {
        var accounts = new string[]
        {
            "fc66cf9b-8fb8-4d9e-ba79-a5e8b87c5aa7",
            "ad518e56-a2d3-46dc-a72b-2cebfac23561"
        };

        var request = new LoadPortfolioSnapshotsRequest
        {
            Accounts = accounts,
        };

        var mediator = ServiceProvider.GetRequiredService<IMediator>();
        await mediator.Send(request, CancellationToken.None);

        var repo = ServiceProvider.GetRequiredService<ITinvestPortfolioRepository>();

        var snapshots0 = await repo.Get(accounts[0]);
        Assert.That(snapshots0.Any(), Is.True);

        foreach (var snapshot in snapshots0)
        {
            Console.WriteLine(snapshot);
        }

        var snapshots1 = await repo.Get(accounts[1]);
        Assert.That(snapshots1.Any(), Is.True);

        foreach (var snapshot in snapshots1)
        {
            Console.WriteLine(snapshot);
        }
    }
}
