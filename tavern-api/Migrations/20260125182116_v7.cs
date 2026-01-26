using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace tavern_api.Migrations
{
    /// <inheritdoc />
    public partial class v7 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Items_Folders_FolderId",
                table: "Items");

            migrationBuilder.AlterColumn<string>(
                name: "FolderId",
                table: "Items",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Items_Folders_FolderId",
                table: "Items",
                column: "FolderId",
                principalTable: "Folders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Items_Folders_FolderId",
                table: "Items");

            migrationBuilder.AlterColumn<string>(
                name: "FolderId",
                table: "Items",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AddForeignKey(
                name: "FK_Items_Folders_FolderId",
                table: "Items",
                column: "FolderId",
                principalTable: "Folders",
                principalColumn: "Id");
        }
    }
}
