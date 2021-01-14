using Microsoft.EntityFrameworkCore.Migrations;

namespace LaundryApi.Migrations
{
    public partial class AddNoOfCustomersandTotalRevenueToLaundriesTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "NoOfCustomers",
                table: "Laundries",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalRevenue",
                table: "Laundries",
                type: "decimal(18,4)",
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NoOfCustomers",
                table: "Laundries");

            migrationBuilder.DropColumn(
                name: "TotalRevenue",
                table: "Laundries");
        }
    }
}
