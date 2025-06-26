using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DIEARD.Migrations
{
    /// <inheritdoc />
    public partial class AddAdminUserAndRole : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "a", null, "Administrador", "ADMINISTRADOR" });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Email", "EmailConfirmed", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { "admin", 0, "9558dc54-63d0-4d43-92cd-7bba56b8f79a", "admin@mail.pt", true, false, null, "ADMIN@MAIL.PT", "ADMIN@MAIL.PT", "AQAAAAIAAYagAAAAEMM/uTvIMKm36S0tW6hvmXdI8jw2u3QmAHLmEoIUQ7K1ssnPDaFMQYby9W+TsUGiFQ==", null, false, "dbf94f91-d2ff-444a-b90c-6c39c1537588", false, "admin@mail.pt" });

            migrationBuilder.UpdateData(
                table: "Categorias",
                keyColumn: "Id",
                keyValue: 1,
                column: "DataCriacao",
                value: new DateTime(2025, 6, 18, 15, 43, 6, 875, DateTimeKind.Local).AddTicks(5208));

            migrationBuilder.UpdateData(
                table: "Categorias",
                keyColumn: "Id",
                keyValue: 2,
                column: "DataCriacao",
                value: new DateTime(2025, 6, 18, 15, 43, 6, 875, DateTimeKind.Local).AddTicks(5212));

            migrationBuilder.UpdateData(
                table: "Categorias",
                keyColumn: "Id",
                keyValue: 3,
                column: "DataCriacao",
                value: new DateTime(2025, 6, 18, 15, 43, 6, 875, DateTimeKind.Local).AddTicks(5216));

            migrationBuilder.UpdateData(
                table: "Categorias",
                keyColumn: "Id",
                keyValue: 4,
                column: "DataCriacao",
                value: new DateTime(2025, 6, 18, 15, 43, 6, 875, DateTimeKind.Local).AddTicks(5220));

            migrationBuilder.UpdateData(
                table: "Categorias",
                keyColumn: "Id",
                keyValue: 5,
                column: "DataCriacao",
                value: new DateTime(2025, 6, 18, 15, 43, 6, 875, DateTimeKind.Local).AddTicks(5224));

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[] { "a", "admin" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "a", "admin" });

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "a");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "admin");

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
    }
}
