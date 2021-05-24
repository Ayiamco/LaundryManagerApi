using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LaundryApi.Entites
{
    public class Location
    {
        public int Id { get; set; }
        public string  State { get; set; }
        public string  LocalGovtArea { get; set; }
        public string  Street { get; set; }
    }
}
