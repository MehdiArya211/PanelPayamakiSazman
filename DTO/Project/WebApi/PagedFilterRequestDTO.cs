using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO.Project.WebApi
{
    /// <summary>
    /// معادل PagedFilterRequest در OpenAPI
    /// </summary>
    public class PagedFilterRequestDTO
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
        public List<FilterRuleDTO>? Filters { get; set; }
        public string? SortBy { get; set; }
        public bool SortDescending { get; set; } = true;
    }
}
