using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DIEARD.Migrations
{
    /// <inheritdoc />
    public partial class ColunaMoodTracker : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MoodTracker",
                table: "Diarios",
                type: "int",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "admin",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "a8dec559-f4a5-45aa-a4c6-eb6940cb2b76", "AQAAAAIAAYagAAAAEGH7d/XqPsvkfNccTexDckNb9Ub75tJiEzblPCTsoHvSnvnlDeRVTckvw2zc2C3wgA==", "fe047836-75a5-4517-8855-a0424ed53e5b" });

            migrationBuilder.UpdateData(
                table: "Categorias",
                keyColumn: "Id",
                keyValue: 1,
                column: "DataCriacao",
                value: new DateTime(2025, 6, 26, 20, 8, 20, 228, DateTimeKind.Local).AddTicks(1437));

            migrationBuilder.UpdateData(
                table: "Categorias",
                keyColumn: "Id",
                keyValue: 2,
                column: "DataCriacao",
                value: new DateTime(2025, 6, 26, 20, 8, 20, 228, DateTimeKind.Local).AddTicks(1441));

            migrationBuilder.UpdateData(
                table: "Categorias",
                keyColumn: "Id",
                keyValue: 3,
                column: "DataCriacao",
                value: new DateTime(2025, 6, 26, 20, 8, 20, 228, DateTimeKind.Local).AddTicks(1444));

            migrationBuilder.UpdateData(
                table: "Categorias",
                keyColumn: "Id",
                keyValue: 4,
                column: "DataCriacao",
                value: new DateTime(2025, 6, 26, 20, 8, 20, 228, DateTimeKind.Local).AddTicks(1447));

            migrationBuilder.UpdateData(
                table: "Categorias",
                keyColumn: "Id",
                keyValue: 5,
                column: "DataCriacao",
                value: new DateTime(2025, 6, 26, 20, 8, 20, 228, DateTimeKind.Local).AddTicks(1450));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MoodTracker",
                table: "Diarios");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "admin",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "3df63a36-a378-4991-8e41-f713acc4a4b7", "AQAAAAIAAYagAAAAEL7nxqANvOSGUa3PFAm23M/cPducENWvyika8/Hv5sIpIU4lnxiRANMDQ2PGrKrj0w==", "53477a41-783a-4c30-afe6-be6a5ce15b49" });

            migrationBuilder.UpdateData(
                table: "Categorias",
                keyColumn: "Id",
                keyValue: 1,
                column: "DataCriacao",
                value: new DateTime(2025, 6, 21, 12, 51, 17, 510, DateTimeKind.Local).AddTicks(7945));

            migrationBuilder.UpdateData(
                table: "Categorias",
                keyColumn: "Id",
                keyValue: 2,
                column: "DataCriacao",
                value: new DateTime(2025, 6, 21, 12, 51, 17, 510, DateTimeKind.Local).AddTicks(7950));

            migrationBuilder.UpdateData(
                table: "Categorias",
                keyColumn: "Id",
                keyValue: 3,
                column: "DataCriacao",
                value: new DateTime(2025, 6, 21, 12, 51, 17, 510, DateTimeKind.Local).AddTicks(7955));

            migrationBuilder.UpdateData(
                table: "Categorias",
                keyColumn: "Id",
                keyValue: 4,
                column: "DataCriacao",
                value: new DateTime(2025, 6, 21, 12, 51, 17, 510, DateTimeKind.Local).AddTicks(7959));

            migrationBuilder.UpdateData(
                table: "Categorias",
                keyColumn: "Id",
                keyValue: 5,
                column: "DataCriacao",
                value: new DateTime(2025, 6, 21, 12, 51, 17, 510, DateTimeKind.Local).AddTicks(7962));
        }
    }
}
