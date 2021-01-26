using Microsoft.EntityFrameworkCore.Migrations;

namespace LaundryApi.Migrations
{
    public partial class seedUserRoles : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("INSERT INTO [dbo].[Roles] ([Id], [Name]) VALUES (N'dd17cb53-2416-439a-3593-08d8beeb7a6a', N'LaundryOwner')");
            migrationBuilder.Sql("INSERT INTO[dbo].[Roles]([Id], [Name]) VALUES(N'61d47b86-34b2-42da-3594-08d8beeb7a6a', N'LaundryEmployee')");
            migrationBuilder.Sql("INSERT INTO[dbo].[Roles] ([Id], [Name]) VALUES(N'c802c039-8f04-4258-3595-08d8beeb7a6a', N'Admin')");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DELETE * FROM [dbo].[Roles]");
        }
    }
}
