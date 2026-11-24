using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OurSpot.Migrations
{
    /// <inheritdoc />
    public partial class AgregarDestinatariosNotificaciones : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DestinatarioId",
                table: "NotificacionesProveedores",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RolDestinatario",
                table: "NotificacionesProveedores",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TipoNotificacion",
                table: "NotificacionesProveedores",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DestinatarioId",
                table: "NotificacionesProveedores");

            migrationBuilder.DropColumn(
                name: "RolDestinatario",
                table: "NotificacionesProveedores");

            migrationBuilder.DropColumn(
                name: "TipoNotificacion",
                table: "NotificacionesProveedores");
        }
    }
}
