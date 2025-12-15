using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO.Project.ServiceClients
{
    public class ServiceClientDetailDTO
    {
        public string ClientId { get; set; }
        public string DisplayName { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
        public string UnitId { get; set; }
        public List<string> RoleIds { get; set; }
    }
}
