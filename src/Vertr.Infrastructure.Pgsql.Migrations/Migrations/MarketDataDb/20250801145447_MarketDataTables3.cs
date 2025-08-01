using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vertr.Infrastructure.Pgsql.Migrations.Migrations.MarketDataDb
{
    /// <inheritdoc />
    public partial class MarketDataTables3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "instrument_kind",
                table: "instruments");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "instrument_kind",
                table: "instruments",
                type: "text",
                nullable: true);
        }
    }
}
