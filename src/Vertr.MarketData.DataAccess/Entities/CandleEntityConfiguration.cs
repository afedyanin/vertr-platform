using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Vertr.MarketData.Contracts;

namespace Vertr.MarketData.DataAccess.Entities;
internal class CandleEntityConfiguration : IEntityTypeConfiguration<Candle>
{
    public void Configure(EntityTypeBuilder<Candle> builder)
    {
        builder.ToTable("candles");
    }
}
