using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Agazaty.Migrations
{
    public partial class V15 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IntializationCheck",
                table: "AspNetUsers",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IntializationCheck",
                table: "AspNetUsers");
        }
    }
}
