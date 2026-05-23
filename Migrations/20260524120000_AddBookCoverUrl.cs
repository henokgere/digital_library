using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace digital_library.Migrations
{
    /// <inheritdoc />
    public partial class AddBookCoverUrl : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CoverUrl",
                table: "Book",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CoverUrl",
                table: "Book");
        }
    }
}
