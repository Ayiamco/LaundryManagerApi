using Microsoft.EntityFrameworkCore.Migrations;

namespace LaundryApi.Migrations
{
    public partial class RemoveConstraintOnCustomersTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropUniqueConstraint(
                name: "AK_Customers_Username_LaundryId",
                table: "Customers");

            migrationBuilder.AlterColumn<string>(
                name: "Username",
                table: "Customers",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Username",
                table: "Customers",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddUniqueConstraint(
                name: "AK_Customers_Username_LaundryId",
                table: "Customers",
                columns: new[] { "Username", "LaundryId" });
        }
    }
}
