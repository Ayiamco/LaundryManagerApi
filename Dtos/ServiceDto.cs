using LaundryApi.Entites;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace LaundryApi.Dtos
{
    public class ServiceDto:ServiceDtoPartial
    {
        public string Name { get; set; }

        public LaundryDto Laundry { get; set; }

        public Guid LaundryId { get; set; }

        public DateTime CreatedAt {get;set;}

    }

    public class ServiceDtoPartial
    {
        public Guid Id { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,4)")]
        public decimal Price { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,4)")]
        public decimal Revenue { get; set; }
    }
}
