using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO.Project.RoleAssignment
{

    /// <summary>
    /// جایگزینی نقش‌های کاربر (PUT users/{id}/roles)
    /// </summary>
    public class ReplaceUserRolesDTO
    {
        public List<Guid> RoleIds { get; set; }
    }
}
