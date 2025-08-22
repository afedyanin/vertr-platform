using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vertr.Infrastructure.Pgsql.Migrations.Migrations.StrategiesDb
{
    /// <inheritdoc />
    public partial class StrategiesRemoveSubAccount : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "sub_account_id",
                table: "trading_signals",
                newName: "portfolio_id");

            migrationBuilder.RenameColumn(
                name: "sub_account_id",
                table: "strategies",
                newName: "portfolio_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "portfolio_id",
                table: "trading_signals",
                newName: "sub_account_id");

            migrationBuilder.RenameColumn(
                name: "portfolio_id",
                table: "strategies",
                newName: "sub_account_id");
        }
    }
}
