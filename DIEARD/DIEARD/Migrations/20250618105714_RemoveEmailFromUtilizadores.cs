using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DIEARD.Migrations
{
    /// <inheritdoc />
    public partial class RemoveEmailFromUtilizadores : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Email",
                table: "Utilizadores");

            migrationBuilder.UpdateData(
                table: "Categorias",
                keyColumn: "Id",
                keyValue: 1,
                column: "DataCriacao",
                value: new DateTime(2025, 6, 18, 11, 57, 13, 428, DateTimeKind.Local).AddTicks(38));

            migrationBuilder.UpdateData(
                table: "Categorias",
                keyColumn: "Id",
                keyValue: 2,
                column: "DataCriacao",
                value: new DateTime(2025, 6, 18, 11, 57, 13, 428, DateTimeKind.Local).AddTicks(43));

            migrationBuilder.UpdateData(
                table: "Categorias",
                keyColumn: "Id",
                keyValue: 3,
                column: "DataCriacao",
                value: new DateTime(2025, 6, 18, 11, 57, 13, 428, DateTimeKind.Local).AddTicks(47));

            migrationBuilder.UpdateData(
                table: "Categorias",
                keyColumn: "Id",
                keyValue: 4,
                column: "DataCriacao",
                value: new DateTime(2025, 6, 18, 11, 57, 13, 428, DateTimeKind.Local).AddTicks(51));

            migrationBuilder.UpdateData(
                table: "Categorias",
                keyColumn: "Id",
                keyValue: 5,
                column: "DataCriacao",
                value: new DateTime(2025, 6, 18, 11, 57, 13, 428, DateTimeKind.Local).AddTicks(55));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "Utilizadores",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "Categorias",
                keyColumn: "Id",
                keyValue: 1,
                column: "DataCriacao",
                value: new DateTime(2025, 6, 18, 11, 48, 6, 195, DateTimeKind.Local).AddTicks(5646));

            migrationBuilder.UpdateData(
                table: "Categorias",
                keyColumn: "Id",
                keyValue: 2,
                column: "DataCriacao",
                value: new DateTime(2025, 6, 18, 11, 48, 6, 195, DateTimeKind.Local).AddTicks(5651));

            migrationBuilder.UpdateData(
                table: "Categorias",
                keyColumn: "Id",
                keyValue: 3,
                column: "DataCriacao",
                value: new DateTime(2025, 6, 18, 11, 48, 6, 195, DateTimeKind.Local).AddTicks(5655));

            migrationBuilder.UpdateData(
                table: "Categorias",
                keyColumn: "Id",
                keyValue: 4,
                column: "DataCriacao",
                value: new DateTime(2025, 6, 18, 11, 48, 6, 195, DateTimeKind.Local).AddTicks(5659));

            migrationBuilder.UpdateData(
                table: "Categorias",
                keyColumn: "Id",
                keyValue: 5,
                column: "DataCriacao",
                value: new DateTime(2025, 6, 18, 11, 48, 6, 195, DateTimeKind.Local).AddTicks(5663));
        }
    }
}
