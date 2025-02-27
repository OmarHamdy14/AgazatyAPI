using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Agazaty.Migrations
{
    public partial class V8 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Holder",
                table: "NormalLeaves",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LeaveStatus",
                table: "NormalLeaves",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "RejectedBy",
                table: "NormalLeaves",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<double>(
                name: "SickLeavesCount",
                table: "AspNetUsers",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Holder",
                table: "NormalLeaves");

            migrationBuilder.DropColumn(
                name: "LeaveStatus",
                table: "NormalLeaves");

            migrationBuilder.DropColumn(
                name: "RejectedBy",
                table: "NormalLeaves");

            migrationBuilder.DropColumn(
                name: "SickLeavesCount",
                table: "AspNetUsers");
        }
    }
}
