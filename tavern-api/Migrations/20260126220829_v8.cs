using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace tavern_api.Migrations
{
    /// <inheritdoc />
    public partial class v8 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ItemUrl",
                table: "Items");

            migrationBuilder.AddColumn<long>(
                name: "ItemSize",
                table: "Items",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0L);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ItemSize",
                table: "Items");

            migrationBuilder.AddColumn<string>(
                name: "ItemUrl",
                table: "Items",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }
    }
}
