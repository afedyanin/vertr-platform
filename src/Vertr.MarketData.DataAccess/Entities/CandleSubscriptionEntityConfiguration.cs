using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vertr.MarketData.Contracts;

namespace Vertr.MarketData.DataAccess.Entities;

internal class CandleSubscriptionEntityConfiguration : IEntityTypeConfiguration<CandleSubscription>
{
    public void Configure(EntityTypeBuilder<CandleSubscription> builder)
    {
        builder.ToTable("candle_subscriptions");

        builder.HasKey(e => e.Id)
            .HasName("candle_subscriptions_pkey");

        builder.Property(e => e.Id)
            .HasColumnName("id")
            .IsRequired();

        builder.Property(e => e.InstrumentId)
            .HasColumnName("instrument_id")
            .IsRequired();

        builder.Property(e => e.Interval)
            .HasColumnName("interval")
            .IsRequired();

        builder.Property(e => e.ExternalStatus)
            .HasColumnName("external_status");

        builder.Property(e => e.ExternalSubscriptionId)
            .HasColumnName("external_subscription_id");

        builder.Property(e => e.Disabled)
            .HasColumnName("disabled");

        builder.Property(e => e.LoadHistory)
            .HasColumnName("load_history");
    }
}
