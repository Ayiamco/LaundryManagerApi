using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace LaundryApi.Migrations
{
    public partial class addLaundryIdColumnToCustomersTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Customers_Employees_EmployeeId",
                table: "Customers");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_Customers_Username_EmployeeId",
                table: "Customers");

            migrationBuilder.DropIndex(
                name: "IX_Customers_EmployeeId",
                table: "Customers");

            migrationBuilder.AddColumn<Guid>(
                name: "LaundryId",
                table: "Customers",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddUniqueConstraint(
                name: "AK_Customers_Username_LaundryId",
                table: "Customers",
                columns: new[] { "Username", "LaundryId" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropUniqueConstraint(
                name: "AK_Customers_Username_LaundryId",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "LaundryId",
                table: "Customers");

            migrationBuilder.AddUniqueConstraint(
                name: "AK_Customers_Username_EmployeeId",
                table: "Customers",
                columns: new[] { "Username", "EmployeeId" });

            migrationBuilder.CreateIndex(
                name: "IX_Customers_EmployeeId",
                table: "Customers",
                column: "EmployeeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Customers_Employees_EmployeeId",
                table: "Customers",
                column: "EmployeeId",
                principalTable: "Employees",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
