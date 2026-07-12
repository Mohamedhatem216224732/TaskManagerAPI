using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TaskManager.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ModifyRefreshTokenTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "RefershToken",
                table: "UserRefreshToken",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "93a2a4e6-3c6a-4e21-bfa5-b6a71b5b5389", "AQAAAAIAAYagAAAAEJTYCIxKbfF+sTlBSxqL6Sf4hZuPz13K+glqJaHBwV7VUPzhN7WfHL0wkQlGLpfvUg==" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RefershToken",
                table: "UserRefreshToken");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "2286d424-f38b-4a52-86dc-744c62c2704a", "AQAAAAIAAYagAAAAEDPLZWzaKHCs/N8/yhy2IIsihq5O7XLeTPtEiZiZ1LnWlNatEAlb0BuCOmyBP6kYxg==" });
        }
    }
}
