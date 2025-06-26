using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DIEARD.Migrations
{
    /// <inheritdoc />
    public partial class AddAmizadesTableFixed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Amizades",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UtilizadorId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    AmigoId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    DataAmizade = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Amizades", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Amizades_AspNetUsers_AmigoId",
                        column: x => x.AmigoId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Amizades_AspNetUsers_UtilizadorId",
                        column: x => x.UtilizadorId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "admin",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "fa6b0c7b-795f-436f-9cb9-46c39784c6c2", "AQAAAAIAAYagAAAAEEmXCoBF1TA4dN0RtLn4XRUfR5vzab3Bf2kMhsmjMvtJ9mXwx1fb8uSH6FG6qS/OHQ==", "18878631-a1c1-4a03-9bdd-28359c73acb3" });

            migrationBuilder.UpdateData(
                table: "Categorias",
                keyColumn: "Id",
                keyValue: 1,
                column: "DataCriacao",
                value: new DateTime(2025, 6, 18, 16, 39, 58, 187, DateTimeKind.Local).AddTicks(962));

            migrationBuilder.UpdateData(
                table: "Categorias",
                keyColumn: "Id",
                keyValue: 2,
                column: "DataCriacao",
                value: new DateTime(2025, 6, 18, 16, 39, 58, 187, DateTimeKind.Local).AddTicks(968));

            migrationBuilder.UpdateData(
                table: "Categorias",
                keyColumn: "Id",
                keyValue: 3,
                column: "DataCriacao",
                value: new DateTime(2025, 6, 18, 16, 39, 58, 187, DateTimeKind.Local).AddTicks(990));

            migrationBuilder.UpdateData(
                table: "Categorias",
                keyColumn: "Id",
                keyValue: 4,
                column: "DataCriacao",
                value: new DateTime(2025, 6, 18, 16, 39, 58, 187, DateTimeKind.Local).AddTicks(1005));

            migrationBuilder.UpdateData(
                table: "Categorias",
                keyColumn: "Id",
                keyValue: 5,
                column: "DataCriacao",
                value: new DateTime(2025, 6, 18, 16, 39, 58, 187, DateTimeKind.Local).AddTicks(1014));

            migrationBuilder.CreateIndex(
                name: "IX_Amizades_AmigoId",
                table: "Amizades",
                column: "AmigoId");

            migrationBuilder.CreateIndex(
                name: "IX_Amizades_UtilizadorId_AmigoId",
                table: "Amizades",
                columns: new[] { "UtilizadorId", "AmigoId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Amizades");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "admin",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "9558dc54-63d0-4d43-92cd-7bba56b8f79a", "AQAAAAIAAYagAAAAEMM/uTvIMKm36S0tW6hvmXdI8jw2u3QmAHLmEoIUQ7K1ssnPDaFMQYby9W+TsUGiFQ==", "dbf94f91-d2ff-444a-b90c-6c39c1537588" });

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
        }
    }
}
