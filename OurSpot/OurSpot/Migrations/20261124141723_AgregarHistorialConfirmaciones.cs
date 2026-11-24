using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OurSpot.Migrations
{
    /// <inheritdoc />
    public partial class AgregarHistorialConfirmaciones : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "HistorialConfirmaciones",
                columns: table => new
                {
                    HistorialConfirmacionId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DetalleId = table.Column<int>(type: "int", nullable: false),
                    ContratacionIdAnterior = table.Column<int>(type: "int", nullable: false),
                    ContratacionIdNueva = table.Column<int>(type: "int", nullable: false),
                    EstadoAnterior = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MotivoRechazo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FechaRespuestaAnterior = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FechaReasignacion = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HistorialConfirmaciones", x => x.HistorialConfirmacionId);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HistorialConfirmaciones");
        }
    }
}
