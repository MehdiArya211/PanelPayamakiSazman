using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO.Project.RoleAssignment
{
    /// <summary>
    /// تخصیص نقش به کاربر (POST assign)
    /// </summary>
    public class AssignRoleToUserDTO
    {
        public Guid RoleId { get; set; }
        public Guid UserId { get; set; }
    }
}
