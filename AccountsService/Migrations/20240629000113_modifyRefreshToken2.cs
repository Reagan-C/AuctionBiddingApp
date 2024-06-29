using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace AccountsService.Migrations
{
    /// <inheritdoc />
    public partial class modifyRefreshToken2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "5ce1174b-cc11-47ad-bae7-cc990298ba94");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "5e8830c4-1030-40f6-a424-31c99844c3fe");

            migrationBuilder.AddColumn<string>(
                name: "Reason",
                table: "RefreshToken",
                type: "text",
                nullable: true);

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "3241fcff-2389-4a59-abff-94f30fb47a08", null, "User", "USER" },
                    { "8a81cb6e-db32-4311-ba98-9c7bf4c349f3", null, "Admin", "ADMIN" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "3241fcff-2389-4a59-abff-94f30fb47a08");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "8a81cb6e-db32-4311-ba98-9c7bf4c349f3");

            migrationBuilder.DropColumn(
                name: "Reason",
                table: "RefreshToken");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "5ce1174b-cc11-47ad-bae7-cc990298ba94", null, "User", "USER" },
                    { "5e8830c4-1030-40f6-a424-31c99844c3fe", null, "Admin", "ADMIN" }
                });
        }
    }
}
