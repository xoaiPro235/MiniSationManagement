using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MiniStationeryManagement.Mvc.Migrations
{
    /// <inheritdoc />
    public partial class AddStationerySafeCrudFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "StationeryItem",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "StationeryItem",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "StationeryItem",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "StationeryItem",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<uint>(
                name: "xmin",
                table: "StationeryItem",
                type: "xid",
                rowVersion: true,
                nullable: false,
                defaultValue: 0u);

            migrationBuilder.UpdateData(
                table: "StationeryItem",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "DeletedAt", "IsDeleted", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 6, 14, 0, 0, 0, 0, DateTimeKind.Utc), null, false, null });

            migrationBuilder.UpdateData(
                table: "StationeryItem",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "DeletedAt", "IsDeleted", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 6, 14, 0, 0, 0, 0, DateTimeKind.Utc), null, false, null });

            migrationBuilder.UpdateData(
                table: "StationeryItem",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedAt", "DeletedAt", "IsDeleted", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 6, 14, 0, 0, 0, 0, DateTimeKind.Utc), null, false, null });

            migrationBuilder.CreateIndex(
                name: "IX_StationeryItem_Sku",
                table: "StationeryItem",
                column: "Sku",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_StationeryItem_Sku",
                table: "StationeryItem");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "StationeryItem");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "StationeryItem");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "StationeryItem");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "StationeryItem");

            migrationBuilder.DropColumn(
                name: "xmin",
                table: "StationeryItem");
        }
    }
}
