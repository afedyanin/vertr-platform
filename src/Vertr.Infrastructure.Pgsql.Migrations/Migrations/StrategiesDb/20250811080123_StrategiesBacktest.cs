using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vertr.Infrastructure.Pgsql.Migrations.Migrations.StrategiesDb
{
    /// <inheritdoc />
    public partial class StrategiesBacktest : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "account_id",
                table: "trading_signals");

            migrationBuilder.DropColumn(
                name: "account_id",
                table: "strategies");

            migrationBuilder.AddColumn<Guid>(
                name: "backtest_id",
                table: "trading_signals",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "backtest_id",
                table: "strategies",
                type: "uuid",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "backtest_id",
                table: "trading_signals");

            migrationBuilder.DropColumn(
                name: "backtest_id",
                table: "strategies");

            migrationBuilder.AddColumn<string>(
                name: "account_id",
                table: "trading_signals",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "account_id",
                table: "strategies",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
