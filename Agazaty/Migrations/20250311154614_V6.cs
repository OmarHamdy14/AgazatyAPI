using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Agazaty.Migrations
{
    public partial class V6 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IntializationCheck",
                table: "AspNetUsers");

            migrationBuilder.RenameColumn(
                name: "HistoryNormalLeavesCount",
                table: "AspNetUsers",
                newName: "YearsOfWork");

            migrationBuilder.AlterColumn<int>(
                name: "SickLeavesCount",
                table: "AspNetUsers",
                type: "int",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.AlterColumn<int>(
                name: "NormalLeavesCount",
                table: "AspNetUsers",
                type: "int",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.AlterColumn<int>(
                name: "CasualLeavesCount",
                table: "AspNetUsers",
                type: "int",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.AddColumn<int>(
                name: "Counts",
                table: "AspNetUsers",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "HowManyDaysFrom81And47",
                table: "AspNetUsers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LeaveSection",
                table: "AspNetUsers",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "NormalLeavesCount_47",
                table: "AspNetUsers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "NormalLeavesCount_81Before1Years",
                table: "AspNetUsers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "NormalLeavesCount_81Before2Years",
                table: "AspNetUsers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "NormalLeavesCount_81Before3Years",
                table: "AspNetUsers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TakenNormalLeavesCount",
                table: "AspNetUsers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TakenNormalLeavesCount_47",
                table: "AspNetUsers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TakenNormalLeavesCount_81Before1Years",
                table: "AspNetUsers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TakenNormalLeavesCount_81Before2Years",
                table: "AspNetUsers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TakenNormalLeavesCount_81Before3Years",
                table: "AspNetUsers",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Counts",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "HowManyDaysFrom81And47",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "LeaveSection",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "NormalLeavesCount_47",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "NormalLeavesCount_81Before1Years",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "NormalLeavesCount_81Before2Years",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "NormalLeavesCount_81Before3Years",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "TakenNormalLeavesCount",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "TakenNormalLeavesCount_47",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "TakenNormalLeavesCount_81Before1Years",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "TakenNormalLeavesCount_81Before2Years",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "TakenNormalLeavesCount_81Before3Years",
                table: "AspNetUsers");

            migrationBuilder.RenameColumn(
                name: "YearsOfWork",
                table: "AspNetUsers",
                newName: "HistoryNormalLeavesCount");

            migrationBuilder.AlterColumn<double>(
                name: "SickLeavesCount",
                table: "AspNetUsers",
                type: "float",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<double>(
                name: "NormalLeavesCount",
                table: "AspNetUsers",
                type: "float",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<double>(
                name: "CasualLeavesCount",
                table: "AspNetUsers",
                type: "float",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<bool>(
                name: "IntializationCheck",
                table: "AspNetUsers",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
