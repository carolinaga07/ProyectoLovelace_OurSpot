using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OurSpot.Migrations
{
    /// <inheritdoc />
    public partial class CorregirPorcentajeGanancia : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EventoDetalles_Contrataciones_ContratacionId",
                table: "EventoDetalles");

            migrationBuilder.DropForeignKey(
                name: "FK_EventoDetalles_Eventos_EventoId",
                table: "EventoDetalles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_EventoDetalles",
                table: "EventoDetalles");

            migrationBuilder.DropColumn(
                name: "PorcentajeGanacia",
                table: "EventoDetalles");

            migrationBuilder.RenameTable(
                name: "EventoDetalles",
                newName: "EventosDetalles");

            migrationBuilder.RenameIndex(
                name: "IX_EventoDetalles_EventoId",
                table: "EventosDetalles",
                newName: "IX_EventosDetalles_EventoId");

            migrationBuilder.RenameIndex(
                name: "IX_EventoDetalles_ContratacionId",
                table: "EventosDetalles",
                newName: "IX_EventosDetalles_ContratacionId");

            migrationBuilder.AddColumn<string>(
                name: "ApplicationUserId",
                table: "Proveedores",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ProveedorId",
                table: "Contrataciones",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EstadoOperativo",
                table: "EventosDetalles",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaActualizacionEstado",
                table: "EventosDetalles",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ObservacionesProveedor",
                table: "EventosDetalles",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_EventosDetalles",
                table: "EventosDetalles",
                column: "DetalleId");

            migrationBuilder.CreateIndex(
                name: "IX_Proveedores_ApplicationUserId",
                table: "Proveedores",
                column: "ApplicationUserId",
                unique: true,
                filter: "[ApplicationUserId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Contrataciones_ProveedorId",
                table: "Contrataciones",
                column: "ProveedorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Contrataciones_Proveedores_ProveedorId",
                table: "Contrataciones",
                column: "ProveedorId",
                principalTable: "Proveedores",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_EventosDetalles_Contrataciones_ContratacionId",
                table: "EventosDetalles",
                column: "ContratacionId",
                principalTable: "Contrataciones",
                principalColumn: "ContratacionId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_EventosDetalles_Eventos_EventoId",
                table: "EventosDetalles",
                column: "EventoId",
                principalTable: "Eventos",
                principalColumn: "EventoId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Proveedores_AspNetUsers_ApplicationUserId",
                table: "Proveedores",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Contrataciones_Proveedores_ProveedorId",
                table: "Contrataciones");

            migrationBuilder.DropForeignKey(
                name: "FK_EventosDetalles_Contrataciones_ContratacionId",
                table: "EventosDetalles");

            migrationBuilder.DropForeignKey(
                name: "FK_EventosDetalles_Eventos_EventoId",
                table: "EventosDetalles");

            migrationBuilder.DropForeignKey(
                name: "FK_Proveedores_AspNetUsers_ApplicationUserId",
                table: "Proveedores");

            migrationBuilder.DropIndex(
                name: "IX_Proveedores_ApplicationUserId",
                table: "Proveedores");

            migrationBuilder.DropIndex(
                name: "IX_Contrataciones_ProveedorId",
                table: "Contrataciones");

            migrationBuilder.DropPrimaryKey(
                name: "PK_EventosDetalles",
                table: "EventosDetalles");

            migrationBuilder.DropColumn(
                name: "ApplicationUserId",
                table: "Proveedores");

            migrationBuilder.DropColumn(
                name: "ProveedorId",
                table: "Contrataciones");

            migrationBuilder.DropColumn(
                name: "EstadoOperativo",
                table: "EventosDetalles");

            migrationBuilder.DropColumn(
                name: "FechaActualizacionEstado",
                table: "EventosDetalles");

            migrationBuilder.DropColumn(
                name: "ObservacionesProveedor",
                table: "EventosDetalles");

            migrationBuilder.RenameTable(
                name: "EventosDetalles",
                newName: "EventoDetalles");

            migrationBuilder.RenameIndex(
                name: "IX_EventosDetalles_EventoId",
                table: "EventoDetalles",
                newName: "IX_EventoDetalles_EventoId");

            migrationBuilder.RenameIndex(
                name: "IX_EventosDetalles_ContratacionId",
                table: "EventoDetalles",
                newName: "IX_EventoDetalles_ContratacionId");

            migrationBuilder.AddColumn<double>(
                name: "PorcentajeGanacia",
                table: "EventoDetalles",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_EventoDetalles",
                table: "EventoDetalles",
                column: "DetalleId");

            migrationBuilder.AddForeignKey(
                name: "FK_EventoDetalles_Contrataciones_ContratacionId",
                table: "EventoDetalles",
                column: "ContratacionId",
                principalTable: "Contrataciones",
                principalColumn: "ContratacionId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_EventoDetalles_Eventos_EventoId",
                table: "EventoDetalles",
                column: "EventoId",
                principalTable: "Eventos",
                principalColumn: "EventoId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
