using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Repositorios.Migrations
{
    /// <inheritdoc />
    public partial class UsuariosYProyectosEnBdd : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Notificacion_Usuario_UsuarioId",
                table: "Notificacion");

            migrationBuilder.DropForeignKey(
                name: "FK_Proyecto_Usuario_AdministradorId",
                table: "Proyecto");

            migrationBuilder.DropForeignKey(
                name: "FK_Recursos_Proyecto_ProyectoAsociadoId",
                table: "Recursos");

            migrationBuilder.DropForeignKey(
                name: "FK_Tarea_Proyecto_ProyectoId",
                table: "Tarea");

            migrationBuilder.DropForeignKey(
                name: "FK_Usuario_Proyecto_ProyectoId",
                table: "Usuario");

            migrationBuilder.DropForeignKey(
                name: "FK_Usuario_Tarea_TareaId",
                table: "Usuario");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Usuario",
                table: "Usuario");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Proyecto",
                table: "Proyecto");

            migrationBuilder.RenameTable(
                name: "Usuario",
                newName: "Usuarios");

            migrationBuilder.RenameTable(
                name: "Proyecto",
                newName: "Proyectos");

            migrationBuilder.RenameIndex(
                name: "IX_Usuario_TareaId",
                table: "Usuarios",
                newName: "IX_Usuarios_TareaId");

            migrationBuilder.RenameIndex(
                name: "IX_Usuario_ProyectoId",
                table: "Usuarios",
                newName: "IX_Usuarios_ProyectoId");

            migrationBuilder.RenameIndex(
                name: "IX_Proyecto_AdministradorId",
                table: "Proyectos",
                newName: "IX_Proyectos_AdministradorId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Usuarios",
                table: "Usuarios",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Proyectos",
                table: "Proyectos",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Notificacion_Usuarios_UsuarioId",
                table: "Notificacion",
                column: "UsuarioId",
                principalTable: "Usuarios",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Proyectos_Usuarios_AdministradorId",
                table: "Proyectos",
                column: "AdministradorId",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Recursos_Proyectos_ProyectoAsociadoId",
                table: "Recursos",
                column: "ProyectoAsociadoId",
                principalTable: "Proyectos",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Tarea_Proyectos_ProyectoId",
                table: "Tarea",
                column: "ProyectoId",
                principalTable: "Proyectos",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Usuarios_Proyectos_ProyectoId",
                table: "Usuarios",
                column: "ProyectoId",
                principalTable: "Proyectos",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Usuarios_Tarea_TareaId",
                table: "Usuarios",
                column: "TareaId",
                principalTable: "Tarea",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Notificacion_Usuarios_UsuarioId",
                table: "Notificacion");

            migrationBuilder.DropForeignKey(
                name: "FK_Proyectos_Usuarios_AdministradorId",
                table: "Proyectos");

            migrationBuilder.DropForeignKey(
                name: "FK_Recursos_Proyectos_ProyectoAsociadoId",
                table: "Recursos");

            migrationBuilder.DropForeignKey(
                name: "FK_Tarea_Proyectos_ProyectoId",
                table: "Tarea");

            migrationBuilder.DropForeignKey(
                name: "FK_Usuarios_Proyectos_ProyectoId",
                table: "Usuarios");

            migrationBuilder.DropForeignKey(
                name: "FK_Usuarios_Tarea_TareaId",
                table: "Usuarios");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Usuarios",
                table: "Usuarios");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Proyectos",
                table: "Proyectos");

            migrationBuilder.RenameTable(
                name: "Usuarios",
                newName: "Usuario");

            migrationBuilder.RenameTable(
                name: "Proyectos",
                newName: "Proyecto");

            migrationBuilder.RenameIndex(
                name: "IX_Usuarios_TareaId",
                table: "Usuario",
                newName: "IX_Usuario_TareaId");

            migrationBuilder.RenameIndex(
                name: "IX_Usuarios_ProyectoId",
                table: "Usuario",
                newName: "IX_Usuario_ProyectoId");

            migrationBuilder.RenameIndex(
                name: "IX_Proyectos_AdministradorId",
                table: "Proyecto",
                newName: "IX_Proyecto_AdministradorId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Usuario",
                table: "Usuario",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Proyecto",
                table: "Proyecto",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Notificacion_Usuario_UsuarioId",
                table: "Notificacion",
                column: "UsuarioId",
                principalTable: "Usuario",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Proyecto_Usuario_AdministradorId",
                table: "Proyecto",
                column: "AdministradorId",
                principalTable: "Usuario",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Recursos_Proyecto_ProyectoAsociadoId",
                table: "Recursos",
                column: "ProyectoAsociadoId",
                principalTable: "Proyecto",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Tarea_Proyecto_ProyectoId",
                table: "Tarea",
                column: "ProyectoId",
                principalTable: "Proyecto",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Usuario_Proyecto_ProyectoId",
                table: "Usuario",
                column: "ProyectoId",
                principalTable: "Proyecto",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Usuario_Tarea_TareaId",
                table: "Usuario",
                column: "TareaId",
                principalTable: "Tarea",
                principalColumn: "Id");
        }
    }
}
