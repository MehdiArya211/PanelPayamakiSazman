using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO.Project.RolePermission
{
    /// <summary>
    /// ViewModel برای مدیریت مجوزهای نقش
    /// </summary>
    public class RolePermissionFormViewModel
    {
        public string RoleId { get; set; }
        public string RoleName { get; set; }
        public List<PermissionDTO> Permissions { get; set; } = new List<PermissionDTO>();
    }
}
