using Dapper;
using Microsoft.EntityFrameworkCore;
using Vertr.Infrastructure.Pgsql;
using Vertr.MarketData.Contracts;
using Vertr.MarketData.Contracts.Interfaces;
using Vertr.MarketData.DataAccess.Entities;

namespace Vertr.MarketData.DataAccess.Repositories;

internal class CandlesRepository : RepositoryBase, ICandlesRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public CandlesRepository(
        IDbContextFactory<MarketDataDbContext> contextFactory,
        IDbConnectionFactory connectionFactory) : base(contextFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<IEnumerable<Candle>> Get(
        Guid instrumentId,
        int? limit = 100,
        CancellationToken cancellationToken = default)
    {
        using var context = await GetDbContext();

        var query = context
            .Candles
            .AsNoTracking()
            .Where(x => x.InstrumentId == instrumentId)
            .OrderByDescending(x => x.TimeUtc);

        if (limit is null)
        {
            return await query.ToArrayAsync(cancellationToken);
        }

        return await query
            .Take(limit.Value)
            .ToArrayAsync(cancellationToken);
    }

    public async Task<Candle?> GetLast(Guid instrumentId)
    {
        using var context = await GetDbContext();

        var item = context
            .Candles
            .AsNoTracking()
            .Where(x => x.InstrumentId == instrumentId)
            .OrderByDescending(x => x.TimeUtc)
            .FirstOrDefault();

        return item;
    }

    public async Task<int> Upsert(IEnumerable<Candle> candles)
    {
        var sql = @$"INSERT INTO {CandleEntityConfiguration.CandlesTableName} (
        time_utc,
        instrument_id,
        open,
        high,
        low,
        close,
        volume
        ) VALUES (
        @TimeUtc,
        @InstrumentId,
        @Open,
        @High,
        @Low,
        @Close,
        @Volume
        ) ON CONFLICT (time_utc, instrument_id) DO UPDATE SET
        open = EXCLUDED.open,
        high = EXCLUDED.high,
        low = EXCLUDED.low,
        close = EXCLUDED.close,
        volume = EXCLUDED.volume";

        using var connection = _connectionFactory.GetConnection();
        connection.Open();
        var txn = connection.BeginTransaction();
        var rowCount = 0;

        try
        {
            foreach (var candle in candles)
            {
                var param = new
                {
                    candle.TimeUtc,
                    candle.InstrumentId,
                    candle.Open,
                    candle.High,
                    candle.Low,
                    candle.Close,
                    candle.Volume
                };

                var res = await connection.ExecuteAsync(sql, param, txn);
                rowCount += res;
            }

            txn.Commit();
        }
        catch
        {
            txn.Rollback();
            throw;
        }

        return rowCount;
    }

    public async Task<int> Delete(Guid instrumentId, DateTime timeBefore)
    {
        var sql = @$"DELETE FROM {CandleEntityConfiguration.CandlesTableName}
        WHERE instrument_id = @instrumentId
        AND time_utc < @timeBefore";

        var param = new
        {
            instrumentId,
            timeBefore
        };

        using var connection = _connectionFactory.GetConnection();
        var res = await connection.ExecuteAsync(sql, param);

        return res;
    }
}
