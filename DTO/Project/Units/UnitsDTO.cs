using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using DTO.Base;

namespace DTO
{
    public class UnitsDTO : EntityDTO
    {
        [JsonPropertyName("code")]
        public string Code { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("isActive")]
        public bool IsActive { get; set; }

        [JsonPropertyName("parentId")]
        public long? ParentId { get; set; }
    }


    public class ApiResponse<T>
    {
        [JsonPropertyName("data")]
        public T Data { get; set; }

        [JsonPropertyName("meta")]
        public object Meta { get; set; }

        [JsonPropertyName("traceId")]
        public string TraceId { get; set; }
    }


    public class UnitSearchRequest
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }

}
