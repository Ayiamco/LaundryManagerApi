using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace LaundryApi.Migrations
{
    public partial class AddLaundryIdColumnToServicesTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "LaundryId",
                table: "Services",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Services_LaundryId",
                table: "Services",
                column: "LaundryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Services_Laundries_LaundryId",
                table: "Services",
                column: "LaundryId",
                principalTable: "Laundries",
                principalColumn: "LaundryId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Services_Laundries_LaundryId",
                table: "Services");

            migrationBuilder.DropIndex(
                name: "IX_Services_LaundryId",
                table: "Services");

            migrationBuilder.DropColumn(
                name: "LaundryId",
                table: "Services");
        }
    }
}
