using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Agazaty.Migrations
{
    public partial class V4 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_NormalLeaves_AspNetUsers_User_ID",
                table: "NormalLeaves");

            migrationBuilder.DropIndex(
                name: "IX_NormalLeaves_User_ID",
                table: "NormalLeaves");

            migrationBuilder.DropColumn(
                name: "User_ID",
                table: "NormalLeaves");

            migrationBuilder.AddColumn<string>(
                name: "UserID",
                table: "NormalLeaves",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_NormalLeaves_UserID",
                table: "NormalLeaves",
                column: "UserID");

            migrationBuilder.AddForeignKey(
                name: "FK_NormalLeaves_AspNetUsers_UserID",
                table: "NormalLeaves",
                column: "UserID",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_NormalLeaves_AspNetUsers_UserID",
                table: "NormalLeaves");

            migrationBuilder.DropIndex(
                name: "IX_NormalLeaves_UserID",
                table: "NormalLeaves");

            migrationBuilder.DropColumn(
                name: "UserID",
                table: "NormalLeaves");

            migrationBuilder.AddColumn<string>(
                name: "User_ID",
                table: "NormalLeaves",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_NormalLeaves_User_ID",
                table: "NormalLeaves",
                column: "User_ID");

            migrationBuilder.AddForeignKey(
                name: "FK_NormalLeaves_AspNetUsers_User_ID",
                table: "NormalLeaves",
                column: "User_ID",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
