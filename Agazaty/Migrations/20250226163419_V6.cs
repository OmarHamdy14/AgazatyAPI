using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Agazaty.Migrations
{
    public partial class V6 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SickLeaves_AspNetUsers_User_ID",
                table: "SickLeaves");

            migrationBuilder.RenameColumn(
                name: "User_ID",
                table: "SickLeaves",
                newName: "UserID");

            migrationBuilder.RenameIndex(
                name: "IX_SickLeaves_User_ID",
                table: "SickLeaves",
                newName: "IX_SickLeaves_UserID");

            migrationBuilder.AlterColumn<string>(
                name: "MedicalCommitteAddress",
                table: "SickLeaves",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "General_ManagerID",
                table: "NormalLeaves",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Direct_ManagerID",
                table: "NormalLeaves",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Coworker_ID",
                table: "NormalLeaves",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_SickLeaves_AspNetUsers_UserID",
                table: "SickLeaves",
                column: "UserID",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SickLeaves_AspNetUsers_UserID",
                table: "SickLeaves");

            migrationBuilder.RenameColumn(
                name: "UserID",
                table: "SickLeaves",
                newName: "User_ID");

            migrationBuilder.RenameIndex(
                name: "IX_SickLeaves_UserID",
                table: "SickLeaves",
                newName: "IX_SickLeaves_User_ID");

            migrationBuilder.AlterColumn<string>(
                name: "MedicalCommitteAddress",
                table: "SickLeaves",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "General_ManagerID",
                table: "NormalLeaves",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Direct_ManagerID",
                table: "NormalLeaves",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Coworker_ID",
                table: "NormalLeaves",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddForeignKey(
                name: "FK_SickLeaves_AspNetUsers_User_ID",
                table: "SickLeaves",
                column: "User_ID",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
