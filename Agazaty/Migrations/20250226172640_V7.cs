using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Agazaty.Migrations
{
    public partial class V7 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PermitLeaveImages_PermitLeaves_LeaveId",
                table: "PermitLeaveImages");

            migrationBuilder.DropIndex(
                name: "IX_PermitLeaveImages_LeaveId",
                table: "PermitLeaveImages");

            migrationBuilder.AddColumn<int>(
                name: "PermitLeaveId",
                table: "PermitLeaveImages",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_PermitLeaveImages_PermitLeaveId",
                table: "PermitLeaveImages",
                column: "PermitLeaveId");

            migrationBuilder.AddForeignKey(
                name: "FK_PermitLeaveImages_PermitLeaves_PermitLeaveId",
                table: "PermitLeaveImages",
                column: "PermitLeaveId",
                principalTable: "PermitLeaves",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PermitLeaveImages_PermitLeaves_PermitLeaveId",
                table: "PermitLeaveImages");

            migrationBuilder.DropIndex(
                name: "IX_PermitLeaveImages_PermitLeaveId",
                table: "PermitLeaveImages");

            migrationBuilder.DropColumn(
                name: "PermitLeaveId",
                table: "PermitLeaveImages");

            migrationBuilder.CreateIndex(
                name: "IX_PermitLeaveImages_LeaveId",
                table: "PermitLeaveImages",
                column: "LeaveId");

            migrationBuilder.AddForeignKey(
                name: "FK_PermitLeaveImages_PermitLeaves_LeaveId",
                table: "PermitLeaveImages",
                column: "LeaveId",
                principalTable: "PermitLeaves",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
