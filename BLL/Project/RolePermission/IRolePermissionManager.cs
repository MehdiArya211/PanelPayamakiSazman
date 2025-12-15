using DTO.Base;
using DTO.Project.RolePermission;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Project.RolePermission
{
    public interface IRolePermissionManager
    {
        /// <summary>
        /// دریافت مجوزهای یک نقش
        /// </summary>
        List<RolePermissionDTO> GetByRole(string roleName);

        /// <summary>
        /// ثبت گروهی مجوزهای نقش
        /// </summary>
        BaseResult Save(string roleName, List<RolePermissionDTO> permissions);
    }
}
