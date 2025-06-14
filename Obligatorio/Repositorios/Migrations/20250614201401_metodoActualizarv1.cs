using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Repositorios.Migrations
{
    /// <inheritdoc />
    public partial class metodoActualizarv1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: 1,
                column: "ContrasenaEncriptada",
                value: "$2a$11$kNTg.z2xOzKB07Tfi3oZn..ZBWnfLblHGu.46lPFCJEofsDPfWqqG");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: 1,
                column: "ContrasenaEncriptada",
                value: "$2a$11$pmFUfeEmBLWPl4OjY7eJm.do471hYa/sI6NXwF0EAxgTsnBON9Adu");
        }
    }
}
