using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OurSpot.Migrations
{
    /// <inheritdoc />
    public partial class NotificacionesEventos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "NotificacionesEventos",
                columns: table => new
                {
                    NotificacionId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EventoId = table.Column<int>(type: "int", nullable: false),
                    NombreEvento = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EstadoAnterior = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EstadoNuevo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Fecha = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Confirmada = table.Column<bool>(type: "bit", nullable: false),
                    FechaConfirmacion = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NotificacionesEventos", x => x.NotificacionId);
                    table.ForeignKey(
                        name: "FK_NotificacionesEventos_Eventos_EventoId",
                        column: x => x.EventoId,
                        principalTable: "Eventos",
                        principalColumn: "EventoId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_NotificacionesEventos_EventoId",
                table: "NotificacionesEventos",
                column: "EventoId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "NotificacionesEventos");
        }
    }
}
