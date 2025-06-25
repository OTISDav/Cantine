using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CantineAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddPhotoAndPriceToMenuColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PhotoUrl",
                table: "Menus",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Prix",
                table: "Menus",
                type: "TEXT",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PhotoUrl",
                table: "Menus");

            migrationBuilder.DropColumn(
                name: "Prix",
                table: "Menus");
        }
    }
}
