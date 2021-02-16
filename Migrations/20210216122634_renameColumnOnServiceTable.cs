using Microsoft.EntityFrameworkCore.Migrations;

namespace LaundryApi.Migrations
{
    public partial class renameColumnOnServiceTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropUniqueConstraint(
                name: "AK_Services_Description_LaundryId",
                table: "Services");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "Services",
                newName: "Name");

            migrationBuilder.AddUniqueConstraint(
                name: "AK_Services_Name_LaundryId",
                table: "Services",
                columns: new[] { "Name", "LaundryId" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropUniqueConstraint(
                name: "AK_Services_Name_LaundryId",
                table: "Services");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Services",
                newName: "Description");

            migrationBuilder.AddUniqueConstraint(
                name: "AK_Services_Description_LaundryId",
                table: "Services",
                columns: new[] { "Description", "LaundryId" });
        }
    }
}
