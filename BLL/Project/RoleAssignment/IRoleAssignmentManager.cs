using DTO.Base;
using DTO.Project.RoleAssignment;
using System;
using System.Collections.Generic;

namespace BLL.Project.RoleAssignment
{
    public interface IRoleAssignmentManager
    {
        BaseResult AssignToUser(AssignRoleToUserDTO model);

        BaseResult ReplaceUserRoles(Guid userId, List<Guid> roleIds);

        BaseResult AssignToUnit(Guid unitId, Guid roleId);
        /// <summary>
        /// دریافت نقش‌های فعلی کاربر (اگر API داشته باشد)
        /// </summary>
        List<Guid> GetUserRoleIds(Guid userId);

    }
}
