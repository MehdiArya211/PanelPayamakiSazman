using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO.Project.RoutePermission
{
    public class RoutePermissionListDTO
    {
        public string RouteKey { get; set; }
        public List<string> Actions { get; set; } = new List<string>();
        public bool RequiresFreshAuth { get; set; }

        // برای نمایش بهتر در UI
        public string DisplayActions => Actions != null ? string.Join(", ", Actions) : "";
        public string RequiresFreshAuthText => RequiresFreshAuth ? "بله" : "خیر";
    }
}
