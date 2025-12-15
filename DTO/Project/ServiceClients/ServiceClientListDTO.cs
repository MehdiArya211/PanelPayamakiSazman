using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO.Project.ServiceClients
{
    public class ServiceClientListDTO
    {
        public string ClientId { get; set; }
        public string DisplayName { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
        public string UnitName { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
