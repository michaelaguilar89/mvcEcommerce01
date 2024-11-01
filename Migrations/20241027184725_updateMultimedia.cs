using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MVCEcommerce.Migrations
{
    /// <inheritdoc />
    public partial class updateMultimedia : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Products_Multimedia_MultimediaId",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_Products_MultimediaId",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "MultimediaId",
                table: "Products");

            migrationBuilder.AddColumn<int>(
                name: "ProductId",
                table: "Multimedia",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Multimedia_ProductId",
                table: "Multimedia",
                column: "ProductId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Multimedia_Products_ProductId",
                table: "Multimedia",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Multimedia_Products_ProductId",
                table: "Multimedia");

            migrationBuilder.DropIndex(
                name: "IX_Multimedia_ProductId",
                table: "Multimedia");

            migrationBuilder.DropColumn(
                name: "ProductId",
                table: "Multimedia");

            migrationBuilder.AddColumn<int>(
                name: "MultimediaId",
                table: "Products",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Products_MultimediaId",
                table: "Products",
                column: "MultimediaId");

            migrationBuilder.AddForeignKey(
                name: "FK_Products_Multimedia_MultimediaId",
                table: "Products",
                column: "MultimediaId",
                principalTable: "Multimedia",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
