using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace LaundryApi.Models
{
    public class UserRole
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public ApplicationUser ApplicationUser { get; set; }

        [Required]
        [ForeignKey("ApplicationUser")]
        public Guid ApplicationUserId  { get; set; }

        public Role Role { get; set; }

        [Required]
        [ForeignKey("Role")]
        public Guid RoleId { get; set; }
    }
}
