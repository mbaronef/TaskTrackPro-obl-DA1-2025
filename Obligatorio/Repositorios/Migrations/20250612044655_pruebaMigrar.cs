using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Repositorios.Migrations
{
    /// <inheritdoc />
    public partial class pruebaMigrar : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Proyectos_Usuarios_AdministradorId",
                table: "Proyectos");

            migrationBuilder.AddForeignKey(
                name: "FK_Proyectos_Usuarios_AdministradorId",
                table: "Proyectos",
                column: "AdministradorId",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Proyectos_Usuarios_AdministradorId",
                table: "Proyectos");

            migrationBuilder.AddForeignKey(
                name: "FK_Proyectos_Usuarios_AdministradorId",
                table: "Proyectos",
                column: "AdministradorId",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
