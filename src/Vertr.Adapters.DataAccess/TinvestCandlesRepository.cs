using System.Data;
using Dapper;
using Vertr.Domain;
using Vertr.Domain.Ports;

namespace Vertr.Adapters.DataAccess;

public class TinvestCandlesRepository : ITinvestCandlesRepository
{
    private readonly IDbConnection _dbConnection;

    private static readonly string _tinvest_candles_table = "tinvest_candles";

    public TinvestCandlesRepository(IDbConnection dbConnection)
    {
        DefaultTypeMap.MatchNamesWithUnderscores = true;

        _dbConnection = dbConnection;
    }

    public Task<IEnumerable<HistoricCandle>> GetCandles(string symbol, CandleInterval interval, DateTime from, DateTime to)
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

        var res = _dbConnection.Query<HistoricCandle>(sql, param);
        return Task.FromResult(res);
    }

    public Task<IEnumerable<HistoricCandle>> GetLastCandles(string symbol, CandleInterval interval, int count = 10, bool completedOnly = true)
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

        var res = _dbConnection.Query<HistoricCandle>(sql, param);
        return Task.FromResult(res);
    }

    public Task<int> InsertCandles(string symbol, CandleInterval interval, IEnumerable<HistoricCandle> candles)
    {
        throw new NotImplementedException();
    }
}
