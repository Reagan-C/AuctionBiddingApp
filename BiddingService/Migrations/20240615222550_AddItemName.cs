using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BiddingService.Migrations
{
    /// <inheritdoc />
    public partial class AddItemName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ItemName",
                table: "Bids",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ItemName",
                table: "Auctions",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ItemName",
                table: "Bids");

            migrationBuilder.DropColumn(
                name: "ItemName",
                table: "Auctions");
        }
    }
}
