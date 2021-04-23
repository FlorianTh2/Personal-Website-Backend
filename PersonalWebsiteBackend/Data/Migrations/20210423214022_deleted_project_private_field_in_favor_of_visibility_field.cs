using Microsoft.EntityFrameworkCore.Migrations;

namespace PersonalWebsiteBackend.Data.Migrations
{
    public partial class deleted_project_private_field_in_favor_of_visibility_field : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Private",
                table: "Projects");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Private",
                table: "Projects",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }
    }
}
