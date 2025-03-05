using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vertr.Adapters.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "tinvest_candles",
                columns: table => new
                {
                    time_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    symbol = table.Column<string>(type: "text", nullable: false),
                    interval = table.Column<int>(type: "integer", nullable: false),
                    open = table.Column<decimal>(type: "numeric", nullable: false),
                    close = table.Column<decimal>(type: "numeric", nullable: false),
                    high = table.Column<decimal>(type: "numeric", nullable: false),
                    low = table.Column<decimal>(type: "numeric", nullable: false),
                    volume = table.Column<long>(type: "bigint", nullable: false),
                    is_completed = table.Column<bool>(type: "boolean", nullable: false),
                    candle_source = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                });

            migrationBuilder.CreateTable(
                name: "tinvest_operations",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    parent_operation_id = table.Column<Guid>(type: "uuid", nullable: true),
                    account_id = table.Column<string>(type: "text", nullable: false),
                    currency = table.Column<string>(type: "text", nullable: false),
                    payment = table.Column<decimal>(type: "numeric", nullable: false),
                    price = table.Column<decimal>(type: "numeric", nullable: false),
                    state = table.Column<int>(type: "integer", nullable: false),
                    quantity = table.Column<long>(type: "bigint", nullable: false),
                    quantity_rest = table.Column<long>(type: "bigint", nullable: false),
                    instrument_type = table.Column<string>(type: "text", nullable: false),
                    date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    type = table.Column<string>(type: "text", nullable: false),
                    operation_type = table.Column<int>(type: "integer", nullable: false),
                    asset_uid = table.Column<Guid>(type: "uuid", nullable: true),
                    position_uid = table.Column<Guid>(type: "uuid", nullable: false),
                    instrument_uid = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("tinvest_operations_pkey", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "tinvest_orders",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    trading_signal_id = table.Column<Guid>(type: "uuid", nullable: true),
                    time_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    account_id = table.Column<string>(type: "text", nullable: false),
                    order_id = table.Column<string>(type: "text", nullable: false),
                    order_request_id = table.Column<string>(type: "text", nullable: false),
                    execution_report_status = table.Column<int>(type: "integer", nullable: false),
                    lots_requested = table.Column<long>(type: "bigint", nullable: false),
                    lots_executed = table.Column<long>(type: "bigint", nullable: false),
                    initial_order_price = table.Column<decimal>(type: "numeric", nullable: false),
                    executed_order_price = table.Column<decimal>(type: "numeric", nullable: false),
                    total_order_amount = table.Column<decimal>(type: "numeric", nullable: false),
                    initial_commission = table.Column<decimal>(type: "numeric", nullable: false),
                    executed_commission = table.Column<decimal>(type: "numeric", nullable: false),
                    direction = table.Column<int>(type: "integer", nullable: false),
                    initial_security_price = table.Column<decimal>(type: "numeric", nullable: false),
                    order_type = table.Column<int>(type: "integer", nullable: false),
                    message = table.Column<string>(type: "text", nullable: false),
                    instrument_uid = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("tinvest_orders_pkey", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "tinvest_portfolio_snapshots",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    time_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    account_id = table.Column<string>(type: "text", nullable: false),
                    total_amount_shares = table.Column<decimal>(type: "numeric", nullable: false),
                    total_amount_bonds = table.Column<decimal>(type: "numeric", nullable: false),
                    total_amount_etf = table.Column<decimal>(type: "numeric", nullable: false),
                    total_amount_currencies = table.Column<decimal>(type: "numeric", nullable: false),
                    total_amount_futures = table.Column<decimal>(type: "numeric", nullable: false),
                    total_amount_options = table.Column<decimal>(type: "numeric", nullable: false),
                    total_amount_sp = table.Column<decimal>(type: "numeric", nullable: false),
                    total_amount_portfolio = table.Column<decimal>(type: "numeric", nullable: false),
                    expected_yield = table.Column<decimal>(type: "numeric", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("tinvest_portfolio_snapshots_pkey", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "trading_signals",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    time_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    symbol = table.Column<string>(type: "text", nullable: false),
                    action = table.Column<int>(type: "integer", nullable: false),
                    candle_interval = table.Column<int>(type: "integer", nullable: false),
                    predictor_type = table.Column<int>(type: "integer", nullable: false),
                    sb3_algo = table.Column<int>(type: "integer", nullable: false),
                    candles_source = table.Column<string>(type: "text", nullable: false),
                    quantity_lots = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("trading_signals_pkey", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "tinvest_portfolio_positions",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    portfolio_snapshot_id = table.Column<Guid>(type: "uuid", nullable: false),
                    instrument_type = table.Column<string>(type: "text", nullable: false),
                    quantity = table.Column<decimal>(type: "numeric", nullable: false),
                    average_position_price = table.Column<decimal>(type: "numeric", nullable: false),
                    expected_yield = table.Column<decimal>(type: "numeric", nullable: false),
                    current_nkd = table.Column<decimal>(type: "numeric", nullable: false),
                    current_price = table.Column<decimal>(type: "numeric", nullable: false),
                    average_position_price_fifo = table.Column<decimal>(type: "numeric", nullable: false),
                    blocked = table.Column<bool>(type: "boolean", nullable: false),
                    blocked_lots = table.Column<decimal>(type: "numeric", nullable: false),
                    position_uid = table.Column<Guid>(type: "uuid", nullable: false),
                    instrument_uid = table.Column<Guid>(type: "uuid", nullable: false),
                    var_margin = table.Column<decimal>(type: "numeric", nullable: false),
                    expected_yield_fifo = table.Column<decimal>(type: "numeric", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("tinvest_portfolio_positions_pkey", x => x.id);
                    table.ForeignKey(
                        name: "tinvest_portfolio_position_snapshot_fk",
                        column: x => x.portfolio_snapshot_id,
                        principalTable: "tinvest_portfolio_snapshots",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "tinvest_candles_unique",
                table: "tinvest_candles",
                columns: new[] { "time_utc", "interval", "symbol" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_tinvest_portfolio_positions_portfolio_snapshot_id",
                table: "tinvest_portfolio_positions",
                column: "portfolio_snapshot_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "tinvest_candles");

            migrationBuilder.DropTable(
                name: "tinvest_operations");

            migrationBuilder.DropTable(
                name: "tinvest_orders");

            migrationBuilder.DropTable(
                name: "tinvest_portfolio_positions");

            migrationBuilder.DropTable(
                name: "trading_signals");

            migrationBuilder.DropTable(
                name: "tinvest_portfolio_snapshots");
        }
    }
}
