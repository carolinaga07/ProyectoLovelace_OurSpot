using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OurSpot.Migrations
{
    /// <inheritdoc />
    public partial class AgregarCalificacionesEventos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Estrella",
                table: "Eventos");

            migrationBuilder.CreateTable(
                name: "CalificacionEventos",
                columns: table => new
                {
                    CalificacionId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EventoId = table.Column<int>(type: "int", nullable: false),
                    ClienteId = table.Column<int>(type: "int", nullable: false),
                    Estrellas = table.Column<int>(type: "int", nullable: false),
                    FechaCalificacion = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CalificacionEventos", x => x.CalificacionId);
                    table.ForeignKey(
                        name: "FK_CalificacionEventos_Clientes_ClienteId",
                        column: x => x.ClienteId,
                        principalTable: "Clientes",
                        principalColumn: "ClienteId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CalificacionEventos_Eventos_EventoId",
                        column: x => x.EventoId,
                        principalTable: "Eventos",
                        principalColumn: "EventoId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CalificacionEventos_ClienteId",
                table: "CalificacionEventos",
                column: "ClienteId");

            migrationBuilder.CreateIndex(
                name: "IX_CalificacionEventos_EventoId_ClienteId",
                table: "CalificacionEventos",
                columns: new[] { "EventoId", "ClienteId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CalificacionEventos");

            migrationBuilder.AddColumn<int>(
                name: "Estrella",
                table: "Eventos",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
