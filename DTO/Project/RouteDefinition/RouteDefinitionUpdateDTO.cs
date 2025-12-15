using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO.Project.RouteDefinition
{
    public class RouteDefinitionUpdateDTO
    {
        public List<string> Actions { get; set; } = new();
        public bool RequiresFreshAuth { get; set; }
    }
}
