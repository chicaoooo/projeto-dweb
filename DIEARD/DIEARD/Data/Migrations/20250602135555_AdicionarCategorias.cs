using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DIEARD.Data.Migrations
{
    /// <inheritdoc />
    public partial class AdicionarCategorias : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Titulo",
                table: "Diarios",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<int>(
                name: "CategoriaId",
                table: "Diarios",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Categorias",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Descricao = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    DataCriacao = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categorias", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Categorias",
                columns: new[] { "Id", "DataCriacao", "Descricao", "Nome" },
                values: new object[,]
                {
                    { 1, new DateTime(2025, 6, 2, 14, 55, 50, 453, DateTimeKind.Local).AddTicks(5513), "Pensamentos e experiências pessoais", "Pessoal" },
                    { 2, new DateTime(2025, 6, 2, 14, 55, 50, 453, DateTimeKind.Local).AddTicks(5564), "Relacionado ao trabalho e carreira", "Trabalho" },
                    { 3, new DateTime(2025, 6, 2, 14, 55, 50, 453, DateTimeKind.Local).AddTicks(5568), "Bem-estar físico e mental", "Saúde" },
                    { 4, new DateTime(2025, 6, 2, 14, 55, 50, 453, DateTimeKind.Local).AddTicks(5570), "Experiências de viagem", "Viagens" },
                    { 5, new DateTime(2025, 6, 2, 14, 55, 50, 453, DateTimeKind.Local).AddTicks(5573), "Momentos em família", "Família" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Diarios_CategoriaId",
                table: "Diarios",
                column: "CategoriaId");

            migrationBuilder.AddForeignKey(
                name: "FK_Diarios_Categorias_CategoriaId",
                table: "Diarios",
                column: "CategoriaId",
                principalTable: "Categorias",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Diarios_Categorias_CategoriaId",
                table: "Diarios");

            migrationBuilder.DropTable(
                name: "Categorias");

            migrationBuilder.DropIndex(
                name: "IX_Diarios_CategoriaId",
                table: "Diarios");

            migrationBuilder.DropColumn(
                name: "CategoriaId",
                table: "Diarios");

            migrationBuilder.AlterColumn<string>(
                name: "Titulo",
                table: "Diarios",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200);
        }
    }
}
