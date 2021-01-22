using Microsoft.EntityFrameworkCore.Migrations;

namespace LaundryApi.Migrations
{
    public partial class RenameColumnInApplicationUsersTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "NoOfUsers",
                table: "ApplicationUsers",
                newName: "NoOfEmployees");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "NoOfEmployees",
                table: "ApplicationUsers",
                newName: "NoOfUsers");
        }
    }
}
