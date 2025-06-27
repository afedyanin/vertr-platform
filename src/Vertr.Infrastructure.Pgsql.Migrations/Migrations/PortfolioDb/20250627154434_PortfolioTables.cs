using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vertr.Infrastructure.Pgsql.Migrations.Migrations.PortfolioDb
{
    /// <inheritdoc />
    public partial class PortfolioTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "operations",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    operation_type = table.Column<int>(type: "integer", nullable: false),
                    order_id = table.Column<string>(type: "text", nullable: true),
                    account_id = table.Column<string>(type: "text", nullable: false),
                    book_id = table.Column<Guid>(type: "uuid", nullable: true),
                    class_code = table.Column<string>(type: "text", nullable: false),
                    ticker = table.Column<string>(type: "text", nullable: false),
                    amount = table.Column<decimal>(type: "numeric", nullable: true),
                    message = table.Column<string>(type: "text", nullable: true),
                    trade_id = table.Column<string>(type: "text", nullable: true),
                    execution_time = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    price = table.Column<decimal>(type: "numeric", nullable: false),
                    quantity = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("operations_pkey", x => x.id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "operations");
        }
    }
}
