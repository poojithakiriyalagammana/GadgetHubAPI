using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GadgetHubAPI.Migrations
{
    public partial class AddQuotationRelationships : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ProductId1",
                table: "Quotations",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Quotations_ProductId1",
                table: "Quotations",
                column: "ProductId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Quotations_Products_ProductId1",
                table: "Quotations",
                column: "ProductId1",
                principalTable: "Products",
                principalColumn: "ProductId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Quotations_Products_ProductId1",
                table: "Quotations");

            migrationBuilder.DropIndex(
                name: "IX_Quotations_ProductId1",
                table: "Quotations");

            migrationBuilder.DropColumn(
                name: "ProductId1",
                table: "Quotations");
        }
    }
}
