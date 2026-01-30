using StackExchange.Redis;

namespace Vertr.TinvestGateway.DataAccess.Redis;

public abstract class RedisRepositoryBase
{
    private readonly IConnectionMultiplexer _connection;

    protected IDatabase GetDatabase() => _connection.GetDatabase();

    protected RedisRepositoryBase(IConnectionMultiplexer connectionMultiplexer)
    {
        _connection = connectionMultiplexer;
    }
}