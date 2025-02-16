using Dapper;
using Vertr.Domain;
using Vertr.Domain.Ports;

namespace Vertr.Adapters.DataAccess;
internal class TradingSignalsRepository : ITradingSignalsRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    private static readonly string _trading_signals_table = "trading_signals";

    public TradingSignalsRepository(IDbConnectionFactory connectionFactory)
    {
        DefaultTypeMap.MatchNamesWithUnderscores = true;

        _connectionFactory = connectionFactory;
    }

    public async Task<IEnumerable<TradingSignal>> Get(string symbol, CandleInterval interval, DateTime from, DateTime to)
    {
        var sql = @$"SELECT * FROM {_trading_signals_table}
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
        var res = await connection.QueryAsync<TradingSignal>(sql, param);

        return res;
    }

    public async Task<IEnumerable<TradingSignal>> GetLast(string symbol, CandleInterval interval, int count = 10)
    {
        var sql = @$"SELECT * FROM {_trading_signals_table}
        WHERE interval = @interval
        AND symbol = @symbol 
        ORDER BY time_utc DESC
        LIMIT @count";

        var param = new
        {
            interval,
            symbol,
            count
        };

        using var connection = _connectionFactory.GetConnection();
        var res = await connection.QueryAsync<TradingSignal>(sql, param);

        return res;
    }

    public Task<int> Insert(IEnumerable<TradingSignal> signals)
    {
        throw new NotImplementedException();
    }

    public async Task<int> Delete(Guid signalId)
    {
        var sql = @$"DELETE FROM {_trading_signals_table}
        WHERE id = @signalId ";

        var param = new
        {
            signalId
        };

        using var connection = _connectionFactory.GetConnection();
        var res = await connection.ExecuteAsync(sql, param);

        return res;
    }

}
