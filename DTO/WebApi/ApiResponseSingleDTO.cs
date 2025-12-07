using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO.WebApi
{
    public class ApiResponseSingleDTO<T>
    {
        public T Data { get; set; }
        public string TraceId { get; set; }
    }
}
