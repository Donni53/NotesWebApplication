using Microsoft.EntityFrameworkCore.Migrations;

namespace NotesWebApplication.Migrations
{
    public partial class deleteTokenField : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                "DeleteToken",
                "Notes",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                "DeleteToken",
                "Notes");
        }
    }
}