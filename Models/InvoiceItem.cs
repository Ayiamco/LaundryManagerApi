using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace LaundryApi.Models
{
    public class InvoiceItem
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }


        public Invoice Invoice { get; set; }

        [Required]
        public Guid InvoiceId { get; set; }

        [Required]
        public Guid ServiceId { get; set; }

        public Service Service { get; set; }

        [Required]
        public int Quantity { get; set; }

    }
}
