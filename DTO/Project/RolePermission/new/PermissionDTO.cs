using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO.Project.RolePermission
{
    public class PermissionDTO
    {
        public string RouteKey { get; set; }
        public List<string> AvailableActions { get; set; }
        public List<string> AssignedActions { get; set; }
    }
}
