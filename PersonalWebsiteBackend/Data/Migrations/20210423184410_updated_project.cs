using Microsoft.EntityFrameworkCore.Migrations;

namespace PersonalWebsiteBackend.Data.Migrations
{
    public partial class updated_project : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "OwnerHTMLURL",
                table: "Projects",
                newName: "OwnerHtmlUrl");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "OwnerHtmlUrl",
                table: "Projects",
                newName: "OwnerHTMLURL");
        }
    }
}
