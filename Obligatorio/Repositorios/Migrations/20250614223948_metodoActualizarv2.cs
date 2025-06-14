using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Repositorios.Migrations
{
    /// <inheritdoc />
    public partial class metodoActualizarv2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Dependencia_Tarea_TareaId",
                table: "Dependencia");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Dependencia",
                table: "Dependencia");

            migrationBuilder.AddColumn<int>(
                name: "TareaDueñaId",
                table: "Dependencia",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Dependencia",
                table: "Dependencia",
                columns: new[] { "TareaDueñaId", "TareaId", "Tipo" });

            migrationBuilder.UpdateData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: 1,
                column: "ContrasenaEncriptada",
                value: "$2a$11$qzN.0MvFRu05/HsgDHERFe6nbH8lCQ3NGpFar4aCDKTqyomMLoYRi");

            migrationBuilder.CreateIndex(
                name: "IX_Dependencia_TareaId",
                table: "Dependencia",
                column: "TareaId");

            migrationBuilder.AddForeignKey(
                name: "FK_Dependencia_Tarea_TareaDueñaId",
                table: "Dependencia",
                column: "TareaDueñaId",
                principalTable: "Tarea",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Dependencia_Tarea_TareaId",
                table: "Dependencia",
                column: "TareaId",
                principalTable: "Tarea",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Dependencia_Tarea_TareaDueñaId",
                table: "Dependencia");

            migrationBuilder.DropForeignKey(
                name: "FK_Dependencia_Tarea_TareaId",
                table: "Dependencia");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Dependencia",
                table: "Dependencia");

            migrationBuilder.DropIndex(
                name: "IX_Dependencia_TareaId",
                table: "Dependencia");

            migrationBuilder.DropColumn(
                name: "TareaDueñaId",
                table: "Dependencia");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Dependencia",
                table: "Dependencia",
                columns: new[] { "TareaId", "Tipo" });

            migrationBuilder.UpdateData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: 1,
                column: "ContrasenaEncriptada",
                value: "$2a$11$kNTg.z2xOzKB07Tfi3oZn..ZBWnfLblHGu.46lPFCJEofsDPfWqqG");

            migrationBuilder.AddForeignKey(
                name: "FK_Dependencia_Tarea_TareaId",
                table: "Dependencia",
                column: "TareaId",
                principalTable: "Tarea",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
