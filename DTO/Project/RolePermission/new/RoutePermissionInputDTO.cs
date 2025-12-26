using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO.Project.RolePermission
{
    public class RoutePermissionInputDTO
    {
        public string RouteKey { get; set; }
        public List<string> Actions { get; set; } = new List<string>();
    }
}
