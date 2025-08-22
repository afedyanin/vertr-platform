using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vertr.Infrastructure.Pgsql.Migrations.Migrations.BacktestDb
{
    /// <inheritdoc />
    public partial class BacktestRemoveSubAccounts2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "sub_account_id",
                table: "backtests",
                newName: "portfolio_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "portfolio_id",
                table: "backtests",
                newName: "sub_account_id");
        }
    }
}
