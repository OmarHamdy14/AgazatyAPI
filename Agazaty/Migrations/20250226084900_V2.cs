using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Agazaty.Migrations
{
    public partial class V2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_Departement_ID",
                table: "AspNetUsers",
                column: "Departement_ID");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Departments_Departement_ID",
                table: "AspNetUsers",
                column: "Departement_ID",
                principalTable: "Departments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Departments_Departement_ID",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_Departement_ID",
                table: "AspNetUsers");
        }
    }
}
