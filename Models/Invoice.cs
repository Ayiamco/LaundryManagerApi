//using System;
//using System.Collections.Generic;
//using System.ComponentModel.DataAnnotations;
//using System.ComponentModel.DataAnnotations.Schema;
//using System.Linq;
//using System.Threading.Tasks;

//namespace LaundryApi.Models
//{
//    public class Invoice
//    {
//        [Key]
//        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
//        public Guid Id { set; get; }

//        [Required]
//        [Column(TypeName = "decimal(18,4)")]
//        public decimal Amount { get; set; }

//        [Required]
//        public DateTime CreatedAt{ get; set; }

//        public Customer Customer { get; set; }

//        [Required]
//        public Guid CustomerId { get; set; }
//    }
//}
