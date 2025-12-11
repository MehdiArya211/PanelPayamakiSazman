using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO.Project.WebApi
{
    /// <summary>
    /// معادل FilterRule در OpenAPI
    /// </summary>
    public class FilterRuleDTO
    {
        public string Field { get; set; }
        public FilterOperator Operator { get; set; }
        public string? Value { get; set; }
    }
}
