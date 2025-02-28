using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Vertr.Application.Operations;
using Vertr.Domain.Repositories;

namespace Vertr.Application.Tests.Operations;

[TestFixture(Category = "System", Explicit = true)]
public class LoadOperationsHandlerTests : ApplicationTestBase
{
    [Test]
    public async Task CanLoadAndSaveOperations()
    {
        var accounts = new string[]
        {
            "fc66cf9b-8fb8-4d9e-ba79-a5e8b87c5aa7",
            //"ad518e56-a2d3-46dc-a72b-2cebfac23561"
        };

        var request = new LoadOperationsRequest
        {
            Accounts = accounts,
        };

        var mediator = ServiceProvider.GetRequiredService<IMediator>();
        await mediator.Send(request, CancellationToken.None);

        var repo = ServiceProvider.GetRequiredService<ITinvestOperationsRepository>();
        var last = await repo.GetLast(accounts[0]);

        Console.WriteLine(last);
    }

    [TestCase("fc66cf9b-8fb8-4d9e-ba79-a5e8b87c5aa7")]
    public async Task CanGetLastOperation(string accountId)
    {
        var repo = ServiceProvider.GetRequiredService<ITinvestOperationsRepository>();

        var last = await repo.GetLast(accountId);

        Assert.That(last, Is.Not.Null);

        Console.WriteLine(last);
    }
}
