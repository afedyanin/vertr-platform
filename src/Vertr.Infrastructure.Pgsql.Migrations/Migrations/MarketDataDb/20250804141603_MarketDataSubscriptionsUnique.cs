using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vertr.Infrastructure.Pgsql.Migrations.Migrations.MarketDataDb
{
    /// <inheritdoc />
    public partial class MarketDataSubscriptionsUnique : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "candle_subscriptions_unique",
                table: "candle_subscriptions",
                columns: new[] { "instrument_id", "interval" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "candle_subscriptions_unique",
                table: "candle_subscriptions");
        }
    }
}
