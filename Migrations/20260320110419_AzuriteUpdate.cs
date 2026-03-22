using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SnackMVCApp.Migrations
{
    /// <inheritdoc />
    public partial class AzuriteUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImageUrl",
                table: "Snacks",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageUrl",
                table: "Snacks");
        }
    }
}
