using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Repositorios.Migrations
{
    /// <inheritdoc />
    public partial class migracionBDD : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: 1,
                column: "ContrasenaEncriptada",
                value: "$2a$11$rETn6ehXRfL3VZXDRdYhl.FgjgSovJngdXlIuzOL/B5kMx8y6Z4/S");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: 1,
                column: "ContrasenaEncriptada",
                value: "$2a$11$iBDuBsQV7PsXk/fruRvfquSAusmBJ6OH8nxp.8Cc7AVoh2bliv/BS");
        }
    }
}
