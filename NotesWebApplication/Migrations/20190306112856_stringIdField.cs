using Microsoft.EntityFrameworkCore.Migrations;

namespace NotesWebApplication.Migrations
{
    public partial class stringIdField : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                "StringId",
                "Notes",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                "StringId",
                "Notes");
        }
    }
}