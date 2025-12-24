using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO.Project.RolePermission
{
    /// <summary>
    /// مدل bulk update برای تمام Routeها
    /// </summary>
    public class RolePermissionBulkUpdateDto
    {
        [Required(ErrorMessage = "نام نقش الزامی است")]
        public string RoleName { get; set; }

        [Required(ErrorMessage = "لیست مجوزها الزامی است")]
        public List<RolePermissionInputDto> Permissions { get; set; } = new List<RolePermissionInputDto>();
    }
}
