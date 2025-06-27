using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vertr.Infrastructure.Pgsql.Migrations.Migrations
{
    /// <inheritdoc />
    public partial class OrderExecutionTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "orders",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    request_id = table.Column<Guid>(type: "uuid", nullable: true),
                    order_id = table.Column<string>(type: "text", nullable: true),
                    class_code = table.Column<string>(type: "text", nullable: false),
                    ticker = table.Column<string>(type: "text", nullable: false),
                    account_id = table.Column<string>(type: "text", nullable: false),
                    book_id = table.Column<Guid>(type: "uuid", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    json_data = table.Column<string>(type: "json", nullable: true),
                    json_data_type = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("orders_pkey", x => x.id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "orders");
        }
    }
}
