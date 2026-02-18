using StackExchange.Redis;

namespace Vertr.Common.Aplication.Tests;

#pragma warning disable CA1063 // Implement IDisposable Correctly
public class RedisPortfoliosTests : IDisposable
#pragma warning restore CA1063 // Implement IDisposable Correctly
{
    private const string PortfoliosKey = "portfolios";
    private const string RedisConnectionString = "localhost";

    private readonly IConnectionMultiplexer _redis;

    private IDatabase GetDatabase() => _redis.GetDatabase();

    public RedisPortfoliosTests()
    {
        _redis = ConnectionMultiplexer.Connect(RedisConnectionString);
    }

    [Test]
    public async Task CanGetRedisPortfolios()
    {
        var entries = await GetDatabase().HashGetAllAsync(PortfoliosKey);

        foreach (var entry in entries)
        {
            Console.WriteLine(entry.Value.ToString());
        }
    }

    [Test]
    public async Task CanClearRedisPortfolios()
    {
        var deleted = await GetDatabase().KeyDeleteAsync(PortfoliosKey);
        Assert.That(deleted, Is.True);
    }

#pragma warning disable CA1816 // Dispose methods should call SuppressFinalize
#pragma warning disable CA1063 // Implement IDisposable Correctly
    public void Dispose() => _redis?.Dispose();
#pragma warning restore CA1063 // Implement IDisposable Correctly
#pragma warning restore CA1816 // Dispose methods should call SuppressFinalize
}
