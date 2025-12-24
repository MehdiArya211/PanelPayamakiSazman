using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO.Project.RolePermission
{
    /// <summary>
    /// مدل نمایش مجوز دسترسی (از API دریافت می‌شود)
    /// </summary>
    public class PermissionDto
    {
        public string RouteKey { get; set; }
        public List<string> AvailableActions { get; set; } = new List<string>();
        public List<string> AssignedActions { get; set; } = new List<string>();

        // برای نمایش در UI
        public string DisplayRouteKey => RouteKey?.Replace(".", "\u2192");
        public bool HasAnyAssigned => AssignedActions != null && AssignedActions.Count > 0;
        public int AssignedCount => AssignedActions?.Count ?? 0;
        public int AvailableCount => AvailableActions?.Count ?? 0;
    }
}
