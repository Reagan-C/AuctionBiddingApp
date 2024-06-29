using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace AccountsService.Migrations
{
    /// <inheritdoc />
    public partial class modifyRefreshToken : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "3cd2693f-58a1-45f6-a9f2-2f156f295c0d");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "5fa9eb4f-7908-430d-a88f-ed849600b980");

            migrationBuilder.AlterColumn<string>(
                name: "RevokedByIp",
                table: "RefreshToken",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "ReplacedByToken",
                table: "RefreshToken",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "5ce1174b-cc11-47ad-bae7-cc990298ba94", null, "User", "USER" },
                    { "5e8830c4-1030-40f6-a424-31c99844c3fe", null, "Admin", "ADMIN" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "5ce1174b-cc11-47ad-bae7-cc990298ba94");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "5e8830c4-1030-40f6-a424-31c99844c3fe");

            migrationBuilder.AlterColumn<string>(
                name: "RevokedByIp",
                table: "RefreshToken",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ReplacedByToken",
                table: "RefreshToken",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "3cd2693f-58a1-45f6-a9f2-2f156f295c0d", null, "User", "USER" },
                    { "5fa9eb4f-7908-430d-a88f-ed849600b980", null, "Admin", "ADMIN" }
                });
        }
    }
}
