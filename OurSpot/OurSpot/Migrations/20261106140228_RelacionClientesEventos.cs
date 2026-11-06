using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OurSpot.Migrations
{
    /// <inheritdoc />
    public partial class RelacionClientesEventos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ClienteId",
                table: "Eventos",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Eventos_ClienteId",
                table: "Eventos",
                column: "ClienteId");

            migrationBuilder.AddForeignKey(
                name: "FK_Eventos_Clientes_ClienteId",
                table: "Eventos",
                column: "ClienteId",
                principalTable: "Clientes",
                principalColumn: "ClienteId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Eventos_Clientes_ClienteId",
                table: "Eventos");

            migrationBuilder.DropIndex(
                name: "IX_Eventos_ClienteId",
                table: "Eventos");

            migrationBuilder.DropColumn(
                name: "ClienteId",
                table: "Eventos");
        }
    }
}
