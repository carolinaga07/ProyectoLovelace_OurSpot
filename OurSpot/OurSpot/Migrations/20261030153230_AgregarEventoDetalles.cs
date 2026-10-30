using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OurSpot.Migrations
{
    /// <inheritdoc />
    public partial class AgregarEventoDetalles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EventoDetalles",
                columns: table => new
                {
                    DetalleId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EventoId = table.Column<int>(type: "int", nullable: false),
                    ContratacionId = table.Column<int>(type: "int", nullable: false),
                    Servicio = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PrecioBase = table.Column<double>(type: "float", nullable: false),
                    PorcentajeGanacia = table.Column<double>(type: "float", nullable: false),
                    PrecioFinal = table.Column<double>(type: "float", nullable: false),
                    PorcentajeGanancia = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventoDetalles", x => x.DetalleId);
                    table.ForeignKey(
                        name: "FK_EventoDetalles_Contrataciones_ContratacionId",
                        column: x => x.ContratacionId,
                        principalTable: "Contrataciones",
                        principalColumn: "ContratacionId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EventoDetalles_Eventos_EventoId",
                        column: x => x.EventoId,
                        principalTable: "Eventos",
                        principalColumn: "EventoId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EventoDetalles_ContratacionId",
                table: "EventoDetalles",
                column: "ContratacionId");

            migrationBuilder.CreateIndex(
                name: "IX_EventoDetalles_EventoId",
                table: "EventoDetalles",
                column: "EventoId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EventoDetalles");
        }
    }
}
