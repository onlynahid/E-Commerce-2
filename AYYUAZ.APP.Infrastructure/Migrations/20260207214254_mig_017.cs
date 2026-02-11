using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AYYUAZ.APP.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class mig_017 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Discounts_Products_Productİd",
                table: "Discounts");

            migrationBuilder.DropIndex(
                name: "IX_Discounts_Productİd",
                table: "Discounts");

            migrationBuilder.DropColumn(
                name: "Productİd",
                table: "Discounts");

            migrationBuilder.CreateIndex(
                name: "IX_Products_DiscountId",
                table: "Products",
                column: "DiscountId",
                unique: true,
                filter: "[DiscountId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_Products_Discounts_DiscountId",
                table: "Products",
                column: "DiscountId",
                principalTable: "Discounts",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Products_Discounts_DiscountId",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_Products_DiscountId",
                table: "Products");

            migrationBuilder.AddColumn<int>(
                name: "Productİd",
                table: "Discounts",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Discounts_Productİd",
                table: "Discounts",
                column: "Productİd",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Discounts_Products_Productİd",
                table: "Discounts",
                column: "Productİd",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
