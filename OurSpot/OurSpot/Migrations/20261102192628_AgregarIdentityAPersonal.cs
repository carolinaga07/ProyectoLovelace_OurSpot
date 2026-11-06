using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OurSpot.Migrations
{
    /// <inheritdoc />
    public partial class AgregarIdentityAPersonal : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ApplicationUserId",
                table: "Personales",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Personales_ApplicationUserId",
                table: "Personales",
                column: "ApplicationUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Personales_AspNetUsers_ApplicationUserId",
                table: "Personales",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Personales_AspNetUsers_ApplicationUserId",
                table: "Personales");

            migrationBuilder.DropIndex(
                name: "IX_Personales_ApplicationUserId",
                table: "Personales");

            migrationBuilder.DropColumn(
                name: "ApplicationUserId",
                table: "Personales");
        }
    }
}
