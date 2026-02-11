using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AYYUAZ.APP.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class mig_011 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Products_Discounts_discountid",
                table: "Products");

            migrationBuilder.DropTable(
                name: "Discounts");

            migrationBuilder.DropIndex(
                name: "IX_Products_discountid",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "discountid",
                table: "Products");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "discountid",
                table: "Products",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Discounts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Percentage = table.Column<decimal>(type: "decimal(18,2)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Discounts", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Products_discountid",
                table: "Products",
                column: "discountid");

            migrationBuilder.AddForeignKey(
                name: "FK_Products_Discounts_discountid",
                table: "Products",
                column: "discountid",
                principalTable: "Discounts",
                principalColumn: "Id");
        }
    }
}
