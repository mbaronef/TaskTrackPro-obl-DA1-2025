using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Repositorios.Migrations
{
    /// <inheritdoc />
    public partial class cambiosAdminInicial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Proyectos_Usuarios_AdministradorId",
                table: "Proyectos");

            migrationBuilder.InsertData(
                table: "Usuarios",
                columns: new[] { "Id", "Apellido", "CantidadProyectosAsignados", "Email", "EsAdministradorProyecto", "EsAdministradorSistema", "EstaAdministrandoUnProyecto", "FechaNacimiento", "Nombre", "ProyectoId", "TareaId", "ContrasenaEncriptada" },
                values: new object[] { 1, "Sistema", 0, "admin@sistema.com", false, true, false, new DateTime(1999, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Admin", null, null, "$2a$11$CsdwIg7/5SHBU7D0hVMtOezIMwVhpfuccs/WKJwQQRakCYzZoDDRm" });

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

            migrationBuilder.DeleteData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: 1);

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
