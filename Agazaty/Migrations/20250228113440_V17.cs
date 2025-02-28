using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Agazaty.Migrations
{
    public partial class V17 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_DepartmentsManagers",
                table: "DepartmentsManagers");

            migrationBuilder.AlterColumn<string>(
                name: "managerid",
                table: "DepartmentsManagers",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "DepartmentsManagers",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DepartmentsManagers",
                table: "DepartmentsManagers",
                column: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_DepartmentsManagers",
                table: "DepartmentsManagers");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "DepartmentsManagers");

            migrationBuilder.AlterColumn<string>(
                name: "managerid",
                table: "DepartmentsManagers",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DepartmentsManagers",
                table: "DepartmentsManagers",
                columns: new[] { "managerid", "departmentId" });
        }
    }
}
