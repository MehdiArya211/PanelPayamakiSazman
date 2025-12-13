using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO.Project.RolePermission
{
    /// <summary>
    /// آیتم مجوز نقش (Route + Actions)
    /// </summary>
    public class RolePermissionDTO
    {
        public string RouteKey { get; set; }

        public List<string> Actions { get; set; } = new();
    }
}
