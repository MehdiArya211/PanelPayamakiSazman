using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO.Project.RolePermission
{
    /// <summary>
    /// ViewModel برای نمایش مجوزهای یک نقش
    /// </summary>
    public class RolePermissionViewModel
    {
        public string RoleId { get; set; }
        public string RoleName { get; set; }
        public List<PermissionDto> Permissions { get; set; } = new List<PermissionDto>();

        // آمار برای نمایش
        public int TotalRoutes => Permissions?.Count ?? 0;
        public int TotalAssignedActions => Permissions?.Sum(p => p.AssignedCount) ?? 0;
        public int TotalAvailableActions => Permissions?.Sum(p => p.AvailableCount) ?? 0;

        // برای فیلتر کردن
        public List<string> AllActions { get; set; } = new List<string>();
    }
}
