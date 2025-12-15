using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO.Project.RoleAssignment
{
    public class RoleAssignmentUserFormViewModel
    {
        public AssignRoleToUserDTO CreateModel { get; set; }

        public List<SelectListItem> UserOptions { get; set; }
        public List<SelectListItem> RoleOptions { get; set; }
    }
}
