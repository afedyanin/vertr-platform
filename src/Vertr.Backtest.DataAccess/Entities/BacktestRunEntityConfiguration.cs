using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vertr.Backtest.Contracts;

namespace Vertr.Backtest.DataAccess.Entities;

internal class BacktestRunEntityConfiguration : IEntityTypeConfiguration<BacktestRun>
{
    public void Configure(EntityTypeBuilder<BacktestRun> builder)
    {
        builder.ToTable("backtest");

        builder.HasKey(e => e.Id)
            .HasName("backtest_pkey");

        builder.Property(e => e.Id)
            .HasColumnName("id")
            .IsRequired();

        builder.Property(e => e.From)
            .HasColumnName("from")
            .IsRequired();

        builder.Property(e => e.To)
            .HasColumnName("to")
            .IsRequired();

        builder.Property(e => e.Description)
            .HasColumnName("description");

        builder.Property(e => e.StrategyId)
            .HasColumnName("strategy_id")
            .IsRequired();

        builder.Property(e => e.InstrumentId)
            .HasColumnName("instrument_id")
            .IsRequired();

        builder.Property(e => e.SubAccountId)
            .HasColumnName("sub_account_id")
            .IsRequired();

        builder.Property(e => e.ExecutionState)
            .HasColumnName("execution_state")
            .IsRequired();

        builder.Property(e => e.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        builder.Property(e => e.UpdatedAt)
            .HasColumnName("updated_time");

        builder.Property(e => e.ProgressMessage)
            .HasColumnName("progress_message");

        builder.Property(e => e.IsCancellationRequested)
            .HasColumnName("is_cancellation_requested")
            .IsRequired();
    }
}
