using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DIEARD.Migrations
{
    /// <inheritdoc />
    public partial class PedidosAmizade : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PedidosAmizade",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RemetenteId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    DestinatarioId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    DataPedido = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DataResposta = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Mensagem = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PedidosAmizade", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PedidosAmizade_AspNetUsers_DestinatarioId",
                        column: x => x.DestinatarioId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PedidosAmizade_AspNetUsers_RemetenteId",
                        column: x => x.RemetenteId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "admin",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "d2760977-852f-49c7-b5a4-c6fb57d12589", "AQAAAAIAAYagAAAAEIqlPN47u+LnbpSetYxG2csP/7bp1+228rYnc/B/TRHdzegeSH210BOtASPzZSYtvA==", "aa943578-8972-41ca-904d-750a8e96ba79" });

            migrationBuilder.UpdateData(
                table: "Categorias",
                keyColumn: "Id",
                keyValue: 1,
                column: "DataCriacao",
                value: new DateTime(2025, 6, 19, 16, 16, 18, 501, DateTimeKind.Local).AddTicks(6975));

            migrationBuilder.UpdateData(
                table: "Categorias",
                keyColumn: "Id",
                keyValue: 2,
                column: "DataCriacao",
                value: new DateTime(2025, 6, 19, 16, 16, 18, 501, DateTimeKind.Local).AddTicks(6980));

            migrationBuilder.UpdateData(
                table: "Categorias",
                keyColumn: "Id",
                keyValue: 3,
                column: "DataCriacao",
                value: new DateTime(2025, 6, 19, 16, 16, 18, 501, DateTimeKind.Local).AddTicks(6984));

            migrationBuilder.UpdateData(
                table: "Categorias",
                keyColumn: "Id",
                keyValue: 4,
                column: "DataCriacao",
                value: new DateTime(2025, 6, 19, 16, 16, 18, 501, DateTimeKind.Local).AddTicks(6989));

            migrationBuilder.UpdateData(
                table: "Categorias",
                keyColumn: "Id",
                keyValue: 5,
                column: "DataCriacao",
                value: new DateTime(2025, 6, 19, 16, 16, 18, 501, DateTimeKind.Local).AddTicks(6993));

            migrationBuilder.CreateIndex(
                name: "IX_PedidosAmizade_DestinatarioId",
                table: "PedidosAmizade",
                column: "DestinatarioId");

            migrationBuilder.CreateIndex(
                name: "IX_PedidosAmizade_RemetenteId_DestinatarioId",
                table: "PedidosAmizade",
                columns: new[] { "RemetenteId", "DestinatarioId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PedidosAmizade");

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
        }
    }
}
