using Dapper;
using Vertr.Domain;
using Vertr.Domain.Enums;
using Vertr.Domain.Ports;

namespace Vertr.Adapters.DataAccess;

internal class TinvestCandlesRepository : ITinvestCandlesRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    private static readonly string _tinvest_candles_table = "tinvest_candles";

    public TinvestCandlesRepository(IDbConnectionFactory connectionFactory)
    {
        DefaultTypeMap.MatchNamesWithUnderscores = true;

        _connectionFactory = connectionFactory;
    }

    public async Task<IEnumerable<HistoricCandle>> Get(string symbol, CandleInterval interval, DateTime from, DateTime to)
    {
        var sql = @$"SELECT * FROM {_tinvest_candles_table}
        WHERE interval = @interval
        AND time_utc >= @from
        AND time_utc <= @to
        AND symbol = @symbol
        ORDER BY time_utc DESC";

        var param = new
        {
            interval,
            from,
            to,
            symbol
        };

        using var connection = _connectionFactory.GetConnection();
        var res = await connection.QueryAsync<HistoricCandle>(sql, param);

        return res;
    }

    public async Task<IEnumerable<HistoricCandle>> GetLast(string symbol, CandleInterval interval, int count = 10, bool completedOnly = true)
    {
        var whereCompleted = completedOnly ? "AND is_completed = true " : " ";
        var sql = @$"SELECT * FROM {_tinvest_candles_table}
        WHERE interval = @interval
        AND symbol = @symbol 
        {whereCompleted}
        ORDER BY time_utc DESC
        LIMIT @count";

        var param = new
        {
            interval,
            symbol,
            count
        };

        using var connection = _connectionFactory.GetConnection();
        var res = await connection.QueryAsync<HistoricCandle>(sql, param);

        return res;
    }

    public async Task<int> Insert(string symbol, CandleInterval interval, IEnumerable<HistoricCandle> candles)
    {
        var sql = @$"INSERT INTO {_tinvest_candles_table}
        (time_utc, interval, symbol, open, high, low, close, volume, is_completed, candle_source)
        VALUES
        (@TimeUtc, @Interval, @Symbol, @Open, @High, @Low, @Close, @Volume, @IsCompleted, @CandleSource)
        ON CONFLICT ON CONSTRAINT tinvest_candles_unique DO UPDATE SET
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
                    Interval = interval,
                    Symbol = symbol,
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

    public async Task<int> Delete(string symbol, CandleInterval interval, DateTime from, DateTime to)
    {
        var sql = @$"DELETE FROM {_tinvest_candles_table}
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
