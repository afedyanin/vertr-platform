using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Vertr.Domain;
using Vertr.Domain.Repositories;

namespace Vertr.Adapters.DataAccess.Tests.Repositories;

[TestFixture(Category = "database", Explicit = true)]
public class TinvestOperationsRepositoryTests
{
    private readonly ServiceProvider _serviceProvider;

    private readonly IConfiguration _configuration;

    protected ITinvestOperationsRepository Repo => _serviceProvider.GetRequiredService<ITinvestOperationsRepository>();

    public TinvestOperationsRepositoryTests()
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
    public async Task CanInsertOperation()
    {
        var operation = new Operation
        {
            Id = Guid.NewGuid(),
            AccountId = "12345",
        };

        var rows = await Repo.Insert(operation);
        Assert.That(rows, Is.GreaterThan(0));
    }

    [Test]
    public async Task CanGetOperations()
    {
        var accountId = "12345";

        var result = await Repo.Get(accountId);

        var operation = result.First();

        Console.WriteLine(operation);
    }

    [Test]
    public async Task CanGetOperationById()
    {
        var id = new Guid("4f2936b3-70c1-43e9-be29-431444dc3001");

        var operation = await Repo.GetById(id);

        Assert.That(operation, Is.Not.Null);

        Console.WriteLine(operation);
    }
}
