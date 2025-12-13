using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO.Project.RouteDefinition
{
    public class RouteDefinitionListDTO
    {
        public string RouteKey { get; set; }
        public List<string> Actions { get; set; }
        public bool RequiresFreshAuth { get; set; }
    }
}
