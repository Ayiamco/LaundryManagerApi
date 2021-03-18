using LaundryApi.Entites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LaundryApi.Dtos
{
    public class DashboardDto
    {
        public string Name { get; set; }
        public int NoOfEmployees { get; set; }
        public int InvoiceCount { get; set; }
        public decimal Revenue { get; set; }
        public decimal InvoiceAmount { get; set; }
        public int CustomerCount { get; set; }
        public string TopService { get;set;}
        
    }
}
