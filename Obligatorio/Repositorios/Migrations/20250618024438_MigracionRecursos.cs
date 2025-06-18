using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Repositorios.Migrations
{
    /// <inheritdoc />
    public partial class MigracionRecursos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RecursoTarea");

            migrationBuilder.AddColumn<int>(
                name: "Capacidad",
                table: "Recursos",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "RangoDeUso",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FechaInicio = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaFin = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CantidadDeUsos = table.Column<int>(type: "int", nullable: false),
                    RecursoId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RangoDeUso", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RangoDeUso_Recursos_RecursoId",
                        column: x => x.RecursoId,
                        principalTable: "Recursos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RecursoNecesario",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RecursoId = table.Column<int>(type: "int", nullable: false),
                    Cantidad = table.Column<int>(type: "int", maxLength: 100, nullable: false),
                    TareaId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecursoNecesario", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RecursoNecesario_Recursos_RecursoId",
                        column: x => x.RecursoId,
                        principalTable: "Recursos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RecursoNecesario_Tarea_TareaId",
                        column: x => x.TareaId,
                        principalTable: "Tarea",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: 1,
                column: "ContrasenaEncriptada",
                value: "$2a$11$iBDuBsQV7PsXk/fruRvfquSAusmBJ6OH8nxp.8Cc7AVoh2bliv/BS");

            migrationBuilder.CreateIndex(
                name: "IX_RangoDeUso_RecursoId",
                table: "RangoDeUso",
                column: "RecursoId");

            migrationBuilder.CreateIndex(
                name: "IX_RecursoNecesario_RecursoId",
                table: "RecursoNecesario",
                column: "RecursoId");

            migrationBuilder.CreateIndex(
                name: "IX_RecursoNecesario_TareaId",
                table: "RecursoNecesario",
                column: "TareaId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RangoDeUso");

            migrationBuilder.DropTable(
                name: "RecursoNecesario");

            migrationBuilder.DropColumn(
                name: "Capacidad",
                table: "Recursos");

            migrationBuilder.CreateTable(
                name: "RecursoTarea",
                columns: table => new
                {
                    RecursosNecesariosId = table.Column<int>(type: "int", nullable: false),
                    TareaId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecursoTarea", x => new { x.RecursosNecesariosId, x.TareaId });
                    table.ForeignKey(
                        name: "FK_RecursoTarea_Recursos_RecursosNecesariosId",
                        column: x => x.RecursosNecesariosId,
                        principalTable: "Recursos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RecursoTarea_Tarea_TareaId",
                        column: x => x.TareaId,
                        principalTable: "Tarea",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: 1,
                column: "ContrasenaEncriptada",
                value: "$2a$11$ODflM2BzPZ1n9l6ml.m9BuvjUFKErRpewBxhwzoPsWS3GUBokc8R6");

            migrationBuilder.CreateIndex(
                name: "IX_RecursoTarea_TareaId",
                table: "RecursoTarea",
                column: "TareaId");
        }
    }
}
