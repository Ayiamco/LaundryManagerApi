using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LaundryApi.Dtos
{
    public class ResponseDto<T>
    {
        public string status { get; set; }
        public string message { get; set; }
        public T data { get; set; }
        public string userRole {get;set;}

    }
}
