using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace LaundryApi.Models
{
    public class ApplicationUser
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [Required]
        public string LaundryName { get; set; }

        [Required]
        public string FullName { get; set; }

        //Username is an alternate key
        [EmailAddress]
        public string Username { get; set; }

        [Required]
        public string PasswordHash { get; set; }

        [Required]
        [StringLength(11)]
        public string PhoneNumber { get; set; }

        [Required]
        public string Address { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,4)")]
        public decimal Revenue { get; set; }

        public DateTime? ForgotPasswordTime { get; set; }

        public string PasswordResetId { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }

        [Required]
        public DateTime UpdatedAt { get; set; }


        //properties below are only fouund in laundryAdmin users

        public int? NoOfCustomers { get; set; }

        public int? NoOfEmployees { get; set; }

       

    }
}
