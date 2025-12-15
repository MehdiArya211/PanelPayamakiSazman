using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO.Project.RolePermission
{
    /// <summary>
    /// DTO ثبت گروهی مجوزهای نقش
    /// </summary>
    public class RolePermissionSaveDTO
    {
        public string RoleName { get; set; }

        public List<RolePermissionDTO> Permissions { get; set; } = new();
    }
}
