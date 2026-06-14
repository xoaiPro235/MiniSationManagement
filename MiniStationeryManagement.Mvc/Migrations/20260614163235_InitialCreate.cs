using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace MiniStationeryManagement.Mvc.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "StationeryCategory",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StationeryCategory", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "StationeryOrder",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    OrderDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    TotalAmount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    CustomerName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StationeryOrder", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "StationeryItem",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Sku = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Name = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    Supplier = table.Column<string>(type: "text", nullable: false),
                    Price = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Quantity = table.Column<int>(type: "integer", nullable: false),
                    MinStock = table.Column<int>(type: "integer", nullable: false),
                    LastUpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CategoryId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StationeryItem", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StationeryItem_StationeryCategory_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "StationeryCategory",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "StationeryOrderItem",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    OrderId = table.Column<int>(type: "integer", nullable: false),
                    StationeryItemId = table.Column<int>(type: "integer", nullable: false),
                    Quantity = table.Column<int>(type: "integer", nullable: false),
                    UnitPrice = table.Column<decimal>(type: "numeric(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StationeryOrderItem", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StationeryOrderItem_StationeryItem_StationeryItemId",
                        column: x => x.StationeryItemId,
                        principalTable: "StationeryItem",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StationeryOrderItem_StationeryOrder_OrderId",
                        column: x => x.OrderId,
                        principalTable: "StationeryOrder",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "StationeryCategory",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Bút & Viết" },
                    { 2, "Giấy & Sổ viết" }
                });

            migrationBuilder.InsertData(
                table: "StationeryItem",
                columns: new[] { "Id", "CategoryId", "LastUpdatedAt", "MinStock", "Name", "Price", "Quantity", "Sku", "Supplier" },
                values: new object[,]
                {
                    { 1, 1, new DateTime(2026, 6, 14, 0, 0, 0, 0, DateTimeKind.Utc), 10, "Bút Bi Thiên Long", 5000m, 50, "VPP-BUT-01", "Tập đoàn Thiên Long" },
                    { 2, 2, new DateTime(2026, 6, 14, 0, 0, 0, 0, DateTimeKind.Utc), 5, "Sổ Lò Xo A5 Hải Tiến", 25000m, 3, "VPP-SO-02", "Công ty Giấy Hải Tiến" },
                    { 3, 2, new DateTime(2026, 6, 14, 0, 0, 0, 0, DateTimeKind.Utc), 5, "Ram Giấy Double A A4", 85000m, 15, "VPP-GIAY-03", "Double A Quốc Tế" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_StationeryItem_CategoryId",
                table: "StationeryItem",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_StationeryOrderItem_OrderId",
                table: "StationeryOrderItem",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_StationeryOrderItem_StationeryItemId",
                table: "StationeryOrderItem",
                column: "StationeryItemId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StationeryOrderItem");

            migrationBuilder.DropTable(
                name: "StationeryItem");

            migrationBuilder.DropTable(
                name: "StationeryOrder");

            migrationBuilder.DropTable(
                name: "StationeryCategory");
        }
    }
}
