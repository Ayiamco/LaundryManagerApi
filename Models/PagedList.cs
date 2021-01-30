using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LaundryApi.Models
{
    public class PagedList <T>
    {
        public IEnumerable<T>  Data { get; set; }

        public int PageSize { get; set; }

        public int PageNumber { get; set; }

        public int OverallSize { get; set; }
    }
}
