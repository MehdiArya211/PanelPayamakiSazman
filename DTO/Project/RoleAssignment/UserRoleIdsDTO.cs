using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO.Project.RoleAssignment
{
    /// <summary>
    /// خروجی استاندارد و ساده برای نقش‌های کاربر
    /// </summary>
    public class UserRoleIdsDTO
    {
        public List<Guid> RoleIds { get; set; } = new();

    }

}
