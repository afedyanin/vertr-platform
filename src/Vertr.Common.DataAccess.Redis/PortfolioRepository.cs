using StackExchange.Redis;
using Vertr.Common.Contracts;
using Vertr.Common.DataAccess.Repositories;

namespace Vertr.Common.DataAccess.Redis;

internal sealed class PortfolioRepository : RedisRepositoryBase, IPortfolioRepository
{
    private const string PortfoliosKey = "portfolios";
    private static readonly RedisChannel PortfolioChannel = new RedisChannel(PortfoliosKey, RedisChannel.PatternMode.Literal);

    public PortfolioRepository(IConnectionMultiplexer connectionMultiplexer) : base(connectionMultiplexer)
    {
    }

    public async Task Save(Portfolio portfolio)
    {
        var db = GetDatabase();
        var json = portfolio.ToJson();
        var portfolioEntry = new HashEntry(portfolio.Id.ToString(), json);

        await Task.WhenAll(
            db.HashSetAsync(PortfoliosKey, [portfolioEntry]),
            db.PublishAsync(PortfolioChannel, json));
    }

    public async Task<Portfolio?> GetById(Guid portfolioId)
    {
        var entry = await GetDatabase().HashGetAsync(PortfoliosKey, portfolioId.ToString());

        if (entry.IsNullOrEmpty)
        {
            return null;
        }

        var restored = Portfolio.FromJson(entry.ToString());
        return restored;
    }
}