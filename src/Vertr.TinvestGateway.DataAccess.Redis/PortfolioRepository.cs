using StackExchange.Redis;
using Vertr.TinvestGateway.Contracts.Portfolios;
using Vertr.TinvestGateway.Repositories;

namespace Vertr.TinvestGateway.DataAccess.Redis;

internal class PortfolioRepository : RedisRepositoryBase, IPortfolioRepository
{
    private const string PortfoliosKey = "portfolios";
    private const string OrderToPortfolioKey = "portfolios.orders";
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

    public async Task BindOrderToPortfolio(string orderId, Guid portfolioId)
    {
        var entry = new HashEntry(orderId, portfolioId.ToString());
        await GetDatabase().HashSetAsync(OrderToPortfolioKey, [entry]);
    }

    public async Task<Guid?> GetPortfolioByOrderId(string orderId)
    {
        var entry = await GetDatabase().HashGetAsync(OrderToPortfolioKey, orderId);

        if (entry.IsNullOrEmpty)
        {
            return null;
        }

        return new Guid(entry.ToString());
    }
}