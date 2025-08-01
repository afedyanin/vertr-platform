using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vertr.MarketData.Contracts;

namespace Vertr.MarketData.DataAccess.Entities;

internal class InstrumentEntityConfiguration : IEntityTypeConfiguration<Instrument>
{
    public void Configure(EntityTypeBuilder<Instrument> builder)
    {
        builder.ToTable("instruments");

        builder.HasKey(e => e.Id)
            .HasName("instruments_pkey");

        builder.Property(e => e.Id)
            .HasColumnName("id")
            .IsRequired();

        var symbol = builder.ComplexProperty(e => e.Symbol).IsRequired();
        symbol.Property(e => e.ClassCode).HasColumnName("symbol_class_code");
        symbol.Property(e => e.Ticker).HasColumnName("symbol_ticker");

        builder.Property(e => e.InstrumentType)
            .HasColumnName("instrument_type");

        builder.Property(e => e.Name)
            .HasColumnName("name")
            .IsRequired();

        builder.Property(e => e.Currency)
            .HasColumnName("currency");

        builder.Property(e => e.LotSize)
            .HasColumnName("lot_size");
    }
}
