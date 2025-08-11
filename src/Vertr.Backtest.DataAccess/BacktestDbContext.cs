using Microsoft.EntityFrameworkCore;
using Vertr.Backtest.Contracts;
using Vertr.Backtest.DataAccess.Entities;

namespace Vertr.Backtest.DataAccess;

public class BacktestDbContext : DbContext
{
    public DbSet<BacktestRun> Backtests { get; set; }

    public BacktestDbContext(DbContextOptions<BacktestDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        new BacktestRunEntityConfiguration().Configure(modelBuilder.Entity<BacktestRun>());
    }
}
