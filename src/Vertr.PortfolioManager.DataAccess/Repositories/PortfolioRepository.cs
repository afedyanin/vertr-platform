using System.Data;
using Dapper;
using Microsoft.EntityFrameworkCore;
using Vertr.PortfolioManager.Contracts;
using Vertr.PortfolioManager.Contracts.Interfaces;
using Vertr.PortfolioManager.DataAccess.Entities;

namespace Vertr.PortfolioManager.DataAccess.Repositories;
internal class PortfolioRepository : RepositoryBase, IPortfolioRepository
{
    public PortfolioRepository(IDbContextFactory<PortfolioDbContext> contextFactory) : base(contextFactory)
    {
    }

    public async Task<Portfolio[]> GetAll()
    {
        using var context = await GetDbContext();

        return await context
            .Portfolios
            .Include(p => p.Positions)
            .AsNoTracking()
            .OrderByDescending(x => x.UpdatedAt)
            .ToArrayAsync();
    }

    public async Task<Portfolio?> GetById(Guid portfolioId)
    {
        using var context = await GetDbContext();

        return context
            .Portfolios
            .Include(p => p.Positions)
            .AsNoTracking()
            .SingleOrDefault(x => x.Id == portfolioId);
    }

    public async Task<bool> Save(Portfolio portfolio)
    {
        var portfolioSql = @$"INSERT INTO {PortfolioEntityConfiguration.PortfoliosTableName} (
        id,
        name,
        updated_at,
        is_backtest
        ) VALUES (
        @Id,
        @Name,
        @UpdatedAt,
        @IsBacktest
        ) ON CONFLICT (id) DO UPDATE SET
        name = EXCLUDED.name,
        updated_at = EXCLUDED.updated_at,
        is_backtest = EXCLUDED.is_backtest";

        var positionSql = @$"INSERT INTO {PositionEntityConfiguration.PositionsTableName} (
        id,
        portfolio_id,
        instrument_id,
        balance
        ) VALUES (
        @Id,
        @PortfolioId,
        @InstrumentId,
        @Balance
        ) ON CONFLICT (id) DO UPDATE SET
        portfolio_id = EXCLUDED.portfolio_id,
        instrument_id = EXCLUDED.instrument_id,
        balance = EXCLUDED.balance";

        using var context = await GetDbContext();
        using var connection = context.Database.GetDbConnection();

        await connection.OpenAsync();
        var txn = await connection.BeginTransactionAsync();
        await context.Database.UseTransactionAsync(txn);

        var rowCount = 0;

        try
        {
            var param = new
            {
                portfolio.Id,
                portfolio.Name,
                portfolio.UpdatedAt,
                portfolio.IsBacktest
            };

            var res = await connection.ExecuteAsync(portfolioSql, param, txn);
            rowCount += res;

            // upsert positions
            foreach (var position in portfolio.Positions)
            {
                var posParam = new
                {
                    position.Id,
                    position.PortfolioId,
                    position.InstrumentId,
                    position.Balance,
                };

                res = await connection.ExecuteAsync(positionSql, posParam, txn);
                rowCount += res;
            }

            // delete positions
            var deletedRows = await DeleteRemovedPositions(context, portfolio);

            await txn.CommitAsync();
        }
        catch
        {
            await txn.RollbackAsync();
            throw;
        }

        return rowCount > 0;
    }

    public async Task<int> Delete(Guid portfolioId)
    {
        using var context = await GetDbContext();

        return await context.Portfolios
            .Where(s => s.Id == portfolioId)
            .ExecuteDeleteAsync();
    }

    private async Task<int> DeleteRemovedPositions(PortfolioDbContext dbContext, Portfolio portfolio)
    {
        var positionIds = portfolio.Positions.Select(s => s.Id).ToArray();

        return await dbContext.Positions
            .Where(p => p.PortfolioId == portfolio.Id && !positionIds.Contains(p.Id))
            .ExecuteDeleteAsync();
    }
}
