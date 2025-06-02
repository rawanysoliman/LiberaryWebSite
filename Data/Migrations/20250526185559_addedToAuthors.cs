using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LibraryManagementSystem.Data.Migrations
{
    /// <inheritdoc />
    public partial class addedToAuthors : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                            name: "Email",
                            table: "Authors",
                            type: "nvarchar(450)",
                            nullable: false,
                            defaultValue: "");
            migrationBuilder.AddColumn<string>(
                name: "Website",
                table: "Authors",
                type: "nvarchar(max)",
                nullable: true);
            migrationBuilder.AddColumn<string>(
                name: "Bio",
                table: "Authors",
                type: "nvarchar(300)",
                maxLength: 300,
                nullable: true);

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Email",
                table: "Authors");
            migrationBuilder.DropColumn(
                name: "Website",
                table: "Authors");
            migrationBuilder.DropColumn(
                name: "Bio",
                table: "Authors");

        }
    }
}
