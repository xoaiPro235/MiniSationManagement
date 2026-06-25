using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MiniStationeryManagement.Mvc.Migrations
{
    /// <inheritdoc />
    public partial class AddBarcodeColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Barcode",
                table: "StationeryItem",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "StationeryItem",
                keyColumn: "Id",
                keyValue: 1,
                column: "Barcode",
                value: "8935212312341");

            migrationBuilder.UpdateData(
                table: "StationeryItem",
                keyColumn: "Id",
                keyValue: 2,
                column: "Barcode",
                value: "8935212312342");

            migrationBuilder.UpdateData(
                table: "StationeryItem",
                keyColumn: "Id",
                keyValue: 3,
                column: "Barcode",
                value: "8935212312343");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Barcode",
                table: "StationeryItem");
        }
    }
}
