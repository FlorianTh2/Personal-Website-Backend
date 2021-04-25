using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace PersonalWebsiteBackend.Data.Migrations
{
    public partial class adjusted_document : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Topic",
                table: "Documents",
                newName: "WebviewLink");

            migrationBuilder.RenameColumn(
                name: "Link",
                table: "Documents",
                newName: "WebcontentLink");

            migrationBuilder.AddColumn<DateTime>(
                name: "DocumentCreatedTime",
                table: "Documents",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "DocumentId",
                table: "Documents",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FileExtension",
                table: "Documents",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FullFileExtension",
                table: "Documents",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Kind",
                table: "Documents",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Md5Checksum",
                table: "Documents",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OwnersEmail",
                table: "Documents",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Shared",
                table: "Documents",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<long>(
                name: "Size",
                table: "Documents",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<string>(
                name: "ThumbnailLink",
                table: "Documents",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "Version",
                table: "Documents",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DocumentCreatedTime",
                table: "Documents");

            migrationBuilder.DropColumn(
                name: "DocumentId",
                table: "Documents");

            migrationBuilder.DropColumn(
                name: "FileExtension",
                table: "Documents");

            migrationBuilder.DropColumn(
                name: "FullFileExtension",
                table: "Documents");

            migrationBuilder.DropColumn(
                name: "Kind",
                table: "Documents");

            migrationBuilder.DropColumn(
                name: "Md5Checksum",
                table: "Documents");

            migrationBuilder.DropColumn(
                name: "OwnersEmail",
                table: "Documents");

            migrationBuilder.DropColumn(
                name: "Shared",
                table: "Documents");

            migrationBuilder.DropColumn(
                name: "Size",
                table: "Documents");

            migrationBuilder.DropColumn(
                name: "ThumbnailLink",
                table: "Documents");

            migrationBuilder.DropColumn(
                name: "Version",
                table: "Documents");

            migrationBuilder.RenameColumn(
                name: "WebviewLink",
                table: "Documents",
                newName: "Topic");

            migrationBuilder.RenameColumn(
                name: "WebcontentLink",
                table: "Documents",
                newName: "Link");
        }
    }
}
