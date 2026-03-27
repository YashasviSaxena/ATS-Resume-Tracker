using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ATS.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCompanyLocationAndExtractedText : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Phone",
                table: "Resumes",
                newName: "FilePath");

            migrationBuilder.RenameColumn(
                name: "FullName",
                table: "Resumes",
                newName: "FileName");

            migrationBuilder.RenameColumn(
                name: "Email",
                table: "Resumes",
                newName: "ExtractedText");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "Resumes",
                newName: "UploadedAt");

            migrationBuilder.AddColumn<string>(
                name: "Company",
                table: "JobDescriptions",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Location",
                table: "JobDescriptions",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Company",
                table: "JobDescriptions");

            migrationBuilder.DropColumn(
                name: "Location",
                table: "JobDescriptions");

            migrationBuilder.RenameColumn(
                name: "UploadedAt",
                table: "Resumes",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "FilePath",
                table: "Resumes",
                newName: "Phone");

            migrationBuilder.RenameColumn(
                name: "FileName",
                table: "Resumes",
                newName: "FullName");

            migrationBuilder.RenameColumn(
                name: "ExtractedText",
                table: "Resumes",
                newName: "Email");
        }
    }
}
