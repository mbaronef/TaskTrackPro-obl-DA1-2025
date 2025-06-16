using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Repositorios.Migrations
{
    /// <inheritdoc />
    public partial class migracionCaminoCritico : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "FechaInicioFijadaManualmente",
                table: "Tarea",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.UpdateData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: 1,
                column: "ContrasenaEncriptada",
                value: "$2a$11$ODflM2BzPZ1n9l6ml.m9BuvjUFKErRpewBxhwzoPsWS3GUBokc8R6");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FechaInicioFijadaManualmente",
                table: "Tarea");

            migrationBuilder.UpdateData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: 1,
                column: "ContrasenaEncriptada",
                value: "$2a$11$GCvufW/DLcfla54qygaAy.RHKV/T.0zGGvuAbHbZdURhIze5DIhNS");
        }
    }
}
