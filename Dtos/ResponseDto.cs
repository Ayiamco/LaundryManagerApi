using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LaundryApi.Dtos
{
    public class ResponseDto<T>
    {
        public string statusCode { get; set; }
        public string message { get; set; }
        public T data { get; set; }

    }
}
