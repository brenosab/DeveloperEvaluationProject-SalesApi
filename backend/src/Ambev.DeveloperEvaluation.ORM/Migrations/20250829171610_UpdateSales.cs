using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ambev.DeveloperEvaluation.ORM.Migrations
{
    /// <inheritdoc />
    public partial class UpdateSales : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ProductName",
                table: "SaleItems",
                newName: "Title");

            migrationBuilder.AddColumn<string>(
                name: "Category",
                table: "SaleItems",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "SaleItems",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Image",
                table: "SaleItems",
                type: "character varying(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "Price",
                table: "SaleItems",
                type: "numeric(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "RatingCount",
                table: "SaleItems",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "RatingRate",
                table: "SaleItems",
                type: "numeric(5,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Category",
                table: "SaleItems");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "SaleItems");

            migrationBuilder.DropColumn(
                name: "Image",
                table: "SaleItems");

            migrationBuilder.DropColumn(
                name: "Price",
                table: "SaleItems");

            migrationBuilder.DropColumn(
                name: "RatingCount",
                table: "SaleItems");

            migrationBuilder.DropColumn(
                name: "RatingRate",
                table: "SaleItems");

            migrationBuilder.RenameColumn(
                name: "Title",
                table: "SaleItems",
                newName: "ProductName");
        }
    }
}
