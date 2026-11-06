using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OurSpot.Migrations
{
    /// <inheritdoc />
    public partial class ActualizarDatosBD : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Contrataciones",
                newName: "ContratacionId");

            migrationBuilder.AddColumn<string>(
                name: "Nombre",
                table: "Lugares",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Nombre",
                table: "Lugares");

            migrationBuilder.RenameColumn(
                name: "ContratacionId",
                table: "Contrataciones",
                newName: "Id");
        }
    }
}
