using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vertr.Infrastructure.Pgsql.Migrations.Migrations
{
    /// <inheritdoc />
    public partial class PortfolioTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "operation_events",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    account_id = table.Column<string>(type: "text", nullable: false),
                    book_id = table.Column<Guid>(type: "uuid", nullable: true),
                    json_data = table.Column<string>(type: "json", nullable: true),
                    json_data_type = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("operation_events_pkey", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "portfolio_snapshots",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    account_id = table.Column<string>(type: "text", nullable: false),
                    book_id = table.Column<Guid>(type: "uuid", nullable: true),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    json_data = table.Column<string>(type: "json", nullable: true),
                    json_data_type = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("portfolio_snapshots_pkey", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "portfolio_positions",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    portfolio_snapshot_id = table.Column<Guid>(type: "uuid", nullable: false),
                    instrument_id = table.Column<Guid>(type: "uuid", nullable: false),
                    balance = table.Column<decimal>(type: "numeric", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("portfolio_positions_pkey", x => x.id);
                    table.ForeignKey(
                        name: "portfolio_position_snapshot_fk",
                        column: x => x.portfolio_snapshot_id,
                        principalTable: "portfolio_snapshots",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_portfolio_positions_portfolio_snapshot_id",
                table: "portfolio_positions",
                column: "portfolio_snapshot_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "operation_events");

            migrationBuilder.DropTable(
                name: "portfolio_positions");

            migrationBuilder.DropTable(
                name: "portfolio_snapshots");
        }
    }
}
