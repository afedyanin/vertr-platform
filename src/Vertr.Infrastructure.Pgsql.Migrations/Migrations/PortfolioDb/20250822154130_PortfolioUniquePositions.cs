using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vertr.Infrastructure.Pgsql.Migrations.Migrations.PortfolioDb
{
    /// <inheritdoc />
    public partial class PortfolioUniquePositions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_positions_portfolio_id",
                table: "positions");

            migrationBuilder.CreateIndex(
                name: "positions_unique",
                table: "positions",
                columns: new[] { "portfolio_id", "instrument_id" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "positions_unique",
                table: "positions");

            migrationBuilder.CreateIndex(
                name: "IX_positions_portfolio_id",
                table: "positions",
                column: "portfolio_id");
        }
    }
}
