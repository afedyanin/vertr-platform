using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vertr.Infrastructure.Pgsql.Migrations.Migrations.MarketDataDb
{
    /// <inheritdoc />
    public partial class MarketDataSubscriptions : Migration
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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "candle_subscriptions");
        }
    }
}
