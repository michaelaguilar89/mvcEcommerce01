using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MVCEcommerce.Migrations
{
    /// <inheritdoc />
    public partial class resolvingImageMOdelBug : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProductdId",
                table: "Images");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ProductdId",
                table: "Images",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }
    }
}
