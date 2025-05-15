using Dapper;
using Microsoft.EntityFrameworkCore;
using Vertr.Adapters.DataAccess.Entities;
using Vertr.Domain;
using Vertr.Domain.Enums;
using Vertr.Domain.Repositories;
using Vertr.Infrastructure.Pgsql;

namespace Vertr.Adapters.DataAccess.Repositories;

internal class TinvestCandlesRepository : RepositoryBase, ITinvestCandlesRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public TinvestCandlesRepository(
        IDbContextFactory<VertrDbContext> contextFactory,
        IDbConnectionFactory connectionFactory) : base(contextFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<IEnumerable<HistoricCandle>> Get(
        string symbol,
        CandleInterval interval,
        DateTime from,
        DateTime to,
        CancellationToken cancellationToken = default)
    {
        using var context = await GetDbContext();

        var candles = await context
            .TinvestCandles
            .AsNoTracking()
            .Where(x =>
            x.Symbol == symbol
            && x.Interval == interval
            && x.TimeUtc >= from
            && x.TimeUtc <= to).ToArrayAsync(cancellationToken);

        return candles;
    }

    public async Task<IEnumerable<HistoricCandle>> GetLast(
        string symbol,
        CandleInterval interval,
        int count = 10,
        bool completedOnly = true,
        CancellationToken cancellationToken = default)
    {
        using var context = await GetDbContext();

        var query = context
            .TinvestCandles
            .AsNoTracking()
            .Where(x =>
            x.Symbol == symbol
            && x.Interval == interval
            );

        if (completedOnly)
        {
            query = query.Where(x => x.IsCompleted == completedOnly);
        }

        var candles = await query
            .OrderByDescending(x => x.TimeUtc)
            .Take(count)
            .ToArrayAsync(cancellationToken);

        return candles;
    }

    public async Task<int> Insert(IEnumerable<HistoricCandle> candles)
    {
        var sql = @$"INSERT INTO {HistoricCandleEntityConfiguration.TinvestCandlesTableName} (
        time_utc,
        interval,
        symbol,
        open,
        high,
        low,
        close,
        volume,
        is_completed,
        candle_source
        ) VALUES (
        @TimeUtc,
        @Interval,
        @Symbol,
        @Open,
        @High,
        @Low,
        @Close,
        @Volume,
        @IsCompleted,
        @CandleSource
        ) ON CONFLICT (time_utc, interval, symbol) DO UPDATE SET
        open = EXCLUDED.open,
        close = EXCLUDED.close,
        high = EXCLUDED.high,
        low = EXCLUDED.low,
        volume = EXCLUDED.volume,
        is_completed = EXCLUDED.is_completed,
        candle_source = EXCLUDED.candle_source";

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
                    candle.Interval,
                    candle.Symbol,
                    candle.TimeUtc,
                    candle.Open,
                    candle.High,
                    candle.Low,
                    candle.Close,
                    candle.Volume,
                    candle.IsCompleted,
                    candle.CandleSource,
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

    public async Task<int> Delete(
        string symbol,
        CandleInterval interval,
        DateTime from,
        DateTime to)
    {
        var sql = @$"DELETE FROM {HistoricCandleEntityConfiguration.TinvestCandlesTableName}
        WHERE interval = @interval
        AND time_utc >= @from
        AND time_utc <= @to
        AND symbol = @symbol";

        var param = new
        {
            interval,
            from,
            to,
            symbol
        };

        using var connection = _connectionFactory.GetConnection();
        var res = await connection.ExecuteAsync(sql, param);

        return res;
    }
}
