using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace LaundryApi.Models
{
    public class Service
    {
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,4)")]
        public decimal Price { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }

        [Required]
        public DateTime UpdatedAt { get; set; }

        public ApplicationUser ApplicationUser { get; set; }

        [Required]
        public Guid ApplicationUserId { get; set; }

        [Required]
        public bool IsDeleted { get; set; }
    }
}

//public partial class seedingApplicationUserRoles : Migration
//{
//    protected override void Up(MigrationBuilder migrationBuilder)
//    {
//        migrationBuilder.Sql("INSERT INTO [dbo].[Roles] ([Id], [Name]) VALUES (N'dd17cb53-2416-439a-3593-08d8beeb7a6a', N'LaundryOwner')");
//        migrationBuilder.Sql("INSERT INTO[dbo].[Roles]([Id], [Name]) VALUES(N'61d47b86-34b2-42da-3594-08d8beeb7a6a', N'LaundryEmployee')");
//        migrationBuilder.Sql("INSERT INTO[dbo].[Roles] ([Id], [Name]) VALUES(N'c802c039-8f04-4258-3595-08d8beeb7a6a', N'Admin')");
//    }

//    protected override void Down(MigrationBuilder migrationBuilder)
//    {
//        migrationBuilder.Sql("DELETE * FROM [dbo].[Roles]");
//    }
//}