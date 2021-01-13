using Microsoft.EntityFrameworkCore.Migrations;

namespace LaundryApi.Migrations
{
    public partial class AddAddressColumnToLaundriesTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Address",
                table: "Laundries",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Address",
                table: "Laundries");
        }
    }
}
