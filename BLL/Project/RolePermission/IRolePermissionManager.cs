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
        /// دریافت مجوزهای یک نقش (Async)
        /// </summary>
        Task<RolePermissionViewModel> GetRolePermissionsAsync(string roleId, string roleName);

        /// <summary>
        /// ذخیره مجوزهای یک نقش (Bulk Update) (Async)
        /// </summary>
        Task<BaseResult> SaveRolePermissionsAsync(RolePermissionBulkUpdateDto model);

        /// <summary>
        /// دریافت لیست تمام Actionهای موجود در سیستم (Async)
        /// </summary>
        Task<List<string>> GetAllSystemActionsAsync();

        /// <summary>
        /// کپی مجوزها از یک نقش به نقش دیگر (Async)
        /// </summary>
        Task<BaseResult> CopyPermissionsAsync(string sourceRoleName, string targetRoleName);

        /// <summary>
        /// ریست کردن مجوزهای یک نقش (Async)
        /// </summary>
        Task<BaseResult> ResetPermissionsAsync(string roleName);
    }
}
