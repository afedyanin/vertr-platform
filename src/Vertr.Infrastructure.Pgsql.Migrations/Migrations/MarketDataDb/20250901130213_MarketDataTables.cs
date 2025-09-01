using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vertr.Infrastructure.Pgsql.Migrations.Migrations.MarketDataDb
{
    /// <inheritdoc />
    public partial class MarketDataTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "candle_subscriptions",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    instrument_id = table.Column<Guid>(type: "uuid", nullable: false),
                    interval = table.Column<int>(type: "integer", nullable: false),
                    external_status = table.Column<string>(type: "text", nullable: true),
                    external_subscription_id = table.Column<string>(type: "text", nullable: true),
                    disabled = table.Column<bool>(type: "boolean", nullable: false),
                    load_history = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("candle_subscriptions_pkey", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "candles",
                columns: table => new
                {
                    instrument_id = table.Column<Guid>(type: "uuid", nullable: false),
                    time_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    open = table.Column<decimal>(type: "numeric", nullable: false),
                    close = table.Column<decimal>(type: "numeric", nullable: false),
                    high = table.Column<decimal>(type: "numeric", nullable: false),
                    low = table.Column<decimal>(type: "numeric", nullable: false),
                    volume = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                });

            migrationBuilder.CreateTable(
                name: "candles_history",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    instrument_id = table.Column<Guid>(type: "uuid", nullable: false),
                    interval = table.Column<int>(type: "integer", nullable: false),
                    day = table.Column<DateOnly>(type: "date", nullable: false),
                    data = table.Column<byte[]>(type: "bytea", nullable: false),
                    count = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("candles_history_pkey", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "instruments",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    instrument_type = table.Column<string>(type: "text", nullable: true),
                    name = table.Column<string>(type: "text", nullable: false),
                    currency = table.Column<string>(type: "text", nullable: true),
                    lot_size = table.Column<decimal>(type: "numeric", nullable: true),
                    symbol_class_code = table.Column<string>(type: "text", nullable: false),
                    symbol_ticker = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("instruments_pkey", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "candle_subscriptions_unique",
                table: "candle_subscriptions",
                columns: new[] { "instrument_id", "interval" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "candles_unique",
                table: "candles",
                columns: new[] { "time_utc", "instrument_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "candles_history_unique",
                table: "candles_history",
                columns: new[] { "instrument_id", "interval", "day" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "candle_subscriptions");

            migrationBuilder.DropTable(
                name: "candles");

            migrationBuilder.DropTable(
                name: "candles_history");

            migrationBuilder.DropTable(
                name: "instruments");
        }
    }
}
