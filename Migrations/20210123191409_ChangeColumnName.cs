using Microsoft.EntityFrameworkCore.Migrations;

namespace LaundryApi.Migrations
{
    public partial class ChangeColumnName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Services",
                table: "Services");

            migrationBuilder.DropColumn(
                name: "TempPassword",
                table: "ApplicationUsers");

            migrationBuilder.AddColumn<string>(
                name: "PasswordResetId",
                table: "ApplicationUsers",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddUniqueConstraint(
                name: "AK_Services_Description_ApplicationUserId",
                table: "Services",
                columns: new[] { "Description", "ApplicationUserId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_Services",
                table: "Services",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationUsers_PasswordResetId",
                table: "ApplicationUsers",
                column: "PasswordResetId",
                unique: true,
                filter: "[PasswordResetId] IS NOT NULL");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropUniqueConstraint(
                name: "AK_Services_Description_ApplicationUserId",
                table: "Services");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Services",
                table: "Services");

            migrationBuilder.DropIndex(
                name: "IX_ApplicationUsers_PasswordResetId",
                table: "ApplicationUsers");

            migrationBuilder.DropColumn(
                name: "PasswordResetId",
                table: "ApplicationUsers");

            migrationBuilder.AddColumn<string>(
                name: "TempPassword",
                table: "ApplicationUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Services",
                table: "Services",
                columns: new[] { "Description", "ApplicationUserId" });
        }
    }
}
