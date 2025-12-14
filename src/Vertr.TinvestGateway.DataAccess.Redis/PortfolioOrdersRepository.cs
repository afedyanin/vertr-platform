using StackExchange.Redis;
using Vertr.Common.DataAccess.Redis;
using Vertr.TinvestGateway.Repositories;

namespace Vertr.TinvestGateway.DataAccess.Redis;

internal class PortfolioOrdersRepository : RedisRepositoryBase, IPortfolioOrdersRepository
{
    private const string OrderToPortfolioKey = "portfolios.orders";

    public PortfolioOrdersRepository(IConnectionMultiplexer connectionMultiplexer) : base(connectionMultiplexer)
    {
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
