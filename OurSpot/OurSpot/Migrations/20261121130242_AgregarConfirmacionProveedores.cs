using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OurSpot.Migrations
{
    /// <inheritdoc />
    public partial class AgregarConfirmacionProveedores : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "EstadoConfirmacion",
                table: "EventosDetalles",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaRespuesta",
                table: "EventosDetalles",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MotivoRechazo",
                table: "EventosDetalles",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaCancelacion",
                table: "Contrataciones",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaRespuesta",
                table: "Contrataciones",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MotivoCancelacion",
                table: "Contrataciones",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MotivoRechazo",
                table: "Contrataciones",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EstadoConfirmacion",
                table: "EventosDetalles");

            migrationBuilder.DropColumn(
                name: "FechaRespuesta",
                table: "EventosDetalles");

            migrationBuilder.DropColumn(
                name: "MotivoRechazo",
                table: "EventosDetalles");

            migrationBuilder.DropColumn(
                name: "FechaCancelacion",
                table: "Contrataciones");

            migrationBuilder.DropColumn(
                name: "FechaRespuesta",
                table: "Contrataciones");

            migrationBuilder.DropColumn(
                name: "MotivoCancelacion",
                table: "Contrataciones");

            migrationBuilder.DropColumn(
                name: "MotivoRechazo",
                table: "Contrataciones");
        }
    }
}
