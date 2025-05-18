using Microsoft.EntityFrameworkCore;
using Vertr.MarketData.Application.Abstractions;
using Vertr.MarketData.Contracts;

namespace Vertr.MarketData.DataAccess.Repositories;

internal class CandlesRepository : RepositoryBase, ICandlesRepository
{
    public CandlesRepository(
        IDbContextFactory<MarketDataDbContext> contextFactory) : base(contextFactory)
    {
    }

    public Task<Candle[]> GetByQuery(CandlesQuery query)
    {
        throw new NotImplementedException();
    }

    public Task<bool> Save(Candle candle)
    {
        throw new NotImplementedException();
    }
    public Task<bool> Delete(string symbol, CandleInterval interval, CandleSource source)
    {
        throw new NotImplementedException();
    }
}
