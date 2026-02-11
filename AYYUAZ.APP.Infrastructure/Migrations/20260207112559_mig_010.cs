using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AYYUAZ.APP.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class mig_010 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Products_Discounts_DiscountId",
                table: "Products");

            migrationBuilder.RenameColumn(
                name: "DiscountId",
                table: "Products",
                newName: "discountid");

            migrationBuilder.RenameIndex(
                name: "IX_Products_DiscountId",
                table: "Products",
                newName: "IX_Products_discountid");

            migrationBuilder.AddForeignKey(
                name: "FK_Products_Discounts_discountid",
                table: "Products",
                column: "discountid",
                principalTable: "Discounts",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Products_Discounts_discountid",
                table: "Products");

            migrationBuilder.RenameColumn(
                name: "discountid",
                table: "Products",
                newName: "DiscountId");

            migrationBuilder.RenameIndex(
                name: "IX_Products_discountid",
                table: "Products",
                newName: "IX_Products_DiscountId");

            migrationBuilder.AddForeignKey(
                name: "FK_Products_Discounts_DiscountId",
                table: "Products",
                column: "DiscountId",
                principalTable: "Discounts",
                principalColumn: "Id");
        }
    }
}
