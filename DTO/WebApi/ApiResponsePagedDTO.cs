using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO.WebApi
{
    public class ApiResponsePagedDTO<T>
    {
        public List<T> Data { get; set; }
        public MetaDataDTO Meta { get; set; }
    }

    public class MetaDataDTO
    {
        public int TotalCount { get; set; }
    }


    public class ApiResponseListDTO<T>
    {
        public T Data { get; set; }
        public object Meta { get; set; }
        public string TraceId { get; set; }
    }

}
