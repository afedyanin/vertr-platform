using Microsoft.EntityFrameworkCore;
using Vertr.Domain;

namespace Vertr.Adapters.DataAccess;

public class VertrDbContext : DbContext
{
    public DbSet<Operation> TinvestOperations { get; set; }

    public DbSet<PortfolioSnapshot> TinvestPortfolios { get; set; }

    public VertrDbContext(DbContextOptions<VertrDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Operation>(entity =>
        {
            entity.ToTable("tinvest_operations");

            entity.HasKey(e => e.Id)
                .HasName("tinvest_operations_pkey");

            entity.Property(e => e.Id)
                .HasColumnName("id");

            entity.Property(e => e.ParentOperationId)
                .HasColumnName("parent_operation_id");

            entity.Property(e => e.Currency)
                .HasColumnName("currency")
                .IsRequired();

            entity.Property(e => e.Payment)
                .HasColumnName("payment");

            entity.Property(e => e.Price)
                .HasColumnName("price");

            entity.Property(e => e.State)
                .HasColumnName("state");

            entity.Property(e => e.Quantity)
                .HasColumnName("quantity");

            entity.Property(e => e.QuantityRest)
                .HasColumnName("quantity_rest");

            entity.Property(e => e.InstrumentType)
                .HasColumnName("instrument_type");

            entity.Property(e => e.Date)
                .HasColumnName("date")
                .IsRequired();

            entity.Property(e => e.Type)
                .HasColumnName("type");

            entity.Property(e => e.OperationType)
                .HasColumnName("operation_type");

            entity.Property(e => e.AssetUid)
                .HasColumnName("asset_uid");

            entity.Property(e => e.PositionUid)
                .HasColumnName("position_uid");

            entity.Property(e => e.InstrumentUid)
                .HasColumnName("instrument_uid");

            entity.Ignore(e => e.OperationTrades);
        });

        modelBuilder.Entity<PortfolioSnapshot>(entity =>
        {
            entity.ToTable("tinvest_portfolio_snapshots");

            entity.HasKey(e => e.Id)
                .HasName("tinvest_portfolio_snapshots_pkey");

            entity.Property(e => e.Id)
                .HasColumnName("id")
                .IsRequired();

            entity.Property(e => e.TimeUtc)
                .HasColumnName("time_utc")
                .IsRequired();

            entity.Property(e => e.AccountId)
                .HasColumnName("account_id")
                .IsRequired();

            entity.Property(e => e.TotalAmountShares)
                .HasColumnName("total_amount_shares");

            entity.Property(e => e.TotalAmountBonds)
                .HasColumnName("total_amount_bonds");

            entity.Property(e => e.TotalAmountEtf)
                .HasColumnName("total_amount_etf");

            entity.Property(e => e.TotalAmountCurrencies)
                .HasColumnName("total_amount_currencies");

            entity.Property(e => e.TotalAmountFutures)
                .HasColumnName("total_amount_futures");

            entity.Property(e => e.TotalAmountOptions)
                .HasColumnName("total_amount_options");

            entity.Property(e => e.TotalAmountSp)
                .HasColumnName("total_amount_sp");

            entity.Property(e => e.TotalAmountPortfolio)
                .HasColumnName("total_amount_portfolio");

            entity.Property(e => e.ExpectedYield)
                .HasColumnName("expected_yield");

            entity.HasMany(e => e.Positions)
                .WithOne(p => p.PortfolioSnapshot);
        });

        modelBuilder.Entity<PortfolioPosition>(entity =>
        {
            entity.ToTable("tinvest_portfolio_positions");

            entity.HasKey(e => e.Id)
                .HasName("tinvest_portfolio_positions_pkey");

            entity.Property(e => e.Id)
                .HasColumnName("id")
                .IsRequired();

            entity.Property(e => e.PortfolioSnapshotId)
                .HasColumnName("portfolio_snapshot_id")
                .IsRequired();

            entity.HasOne(e => e.PortfolioSnapshot)
                .WithMany()
                .HasForeignKey(e => e.PortfolioSnapshotId)
                .IsRequired();

            entity.Property(e => e.InstrumentType)
                .HasColumnName("instrument_type");

            entity.Property(e => e.Quantity)
                .HasColumnName("quantity");

            entity.Property(e => e.AveragePositionPrice)
                .HasColumnName("average_position_price");

            entity.Property(e => e.ExpectedYield)
                .HasColumnName("expected_yield");

            entity.Property(e => e.CurrentNkd)
                .HasColumnName("current_nkd");

            entity.Property(e => e.CurrentPrice)
                .HasColumnName("current_price");

            entity.Property(e => e.AveragePositionPriceFifo)
                .HasColumnName("average_position_price_fifo");

            entity.Property(e => e.Blocked)
                .HasColumnName("blocked");

            entity.Property(e => e.BlockedLots)
                .HasColumnName("blocked_lots");

            entity.Property(e => e.PositionUid)
                .HasColumnName("position_uid");

            entity.Property(e => e.InstrumentUid)
                .HasColumnName("instrument_uid");

            entity.Property(e => e.VarMargin)
                .HasColumnName("var_margin");

            entity.Property(e => e.ExpectedYieldFifo)
                .HasColumnName("expected_yield_fifo");
        });

    }
}
