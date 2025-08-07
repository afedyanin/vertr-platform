using Dapper;
using Microsoft.EntityFrameworkCore;
using Vertr.Infrastructure.Pgsql;
using Vertr.MarketData.Contracts;
using Vertr.MarketData.Contracts.Interfaces;
using Vertr.MarketData.DataAccess.Entities;

namespace Vertr.MarketData.DataAccess.Repositories;

internal class CandlesHistoryRepository : RepositoryBase, ICandlesHistoryRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public CandlesHistoryRepository(
        IDbContextFactory<MarketDataDbContext> contextFactory,
        IDbConnectionFactory connectionFactory) : base(contextFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<CandlesHistoryItem[]> Get(Guid instrumentId)
    {
        var sql = @$"
        SELECT
            id,
            instrument_id,
            interval,
            day,
            count
        FROM {CandleHistoryEntityConfiguration.CandlesHistoryTableName}
        WHERE instrument_id=@instrumentId";

        using var connection = _connectionFactory.GetConnection();
        connection.Open();

        var param = new
        {
            instrumentId,
        };

        var res = await connection.QueryAsync<CandlesHistoryItem>(sql, param);

        return [.. res];
    }

    public async Task<CandlesHistoryItem?> GetById(Guid id)
    {
        using var context = await GetDbContext();

        return await context
            .CandlesHistory
            .SingleOrDefaultAsync(x => x.Id == id);
    }

    public async Task<bool> Save(CandlesHistoryItem item)
    {
        var sql = @$"INSERT INTO {CandleHistoryEntityConfiguration.CandlesHistoryTableName} (
        id,
        instrument_id,
        interval,
        day,
        data,
        count
        ) VALUES (
        @Id,
        @InstrumentId,
        @Interval,
        @day,
        @Data,
        @Count
        ) ON CONFLICT (instrument_id, interval, day) DO UPDATE SET
        data = EXCLUDED.data,
        count = EXCLUDED.count";

        using var connection = _connectionFactory.GetConnection();
        connection.Open();
        var txn = connection.BeginTransaction();

        try
        {
            var day = item.Day.ToDateTime(TimeOnly.MinValue);
            var param = new
            {
                item.Id,
                item.InstrumentId,
                item.Interval,
                day,
                item.Data,
                item.Count
            };

            _ = await connection.ExecuteAsync(sql, param, txn);

            txn.Commit();
            return true;
        }
        catch
        {
            txn.Rollback();
            //throw;
            return false;
        }
    }

    public async Task<int> Delete(Guid Id)
    {
        using var context = await GetDbContext();

        return await context.CandlesHistory
            .Where(s => s.Id == Id)
            .ExecuteDeleteAsync();
    }

    public async Task<int> DeleteAll(Guid instrumentId, DateOnly dayBefore)
    {
        using var context = await GetDbContext();

        var sql = @$"DELETE FROM {CandleHistoryEntityConfiguration.CandlesHistoryTableName}
        WHERE instrument_id = @instrumentId
        AND day < @timeBefore";

        var param = new
        {
            instrumentId,
            dayBefore
        };

        using var connection = _connectionFactory.GetConnection();
        var res = await connection.ExecuteAsync(sql, param);

        return res;
    }
}
