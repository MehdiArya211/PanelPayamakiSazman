using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO.Project.ServiceClients
{
    public class ServiceClientCreateDTO
    {
        public string UnitId { get; set; }   // uuid
        public string DisplayName { get; set; }
        public string Description { get; set; }
        public List<string> RoleIds { get; set; } = new();
    }
}
