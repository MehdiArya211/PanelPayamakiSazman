using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO.Project.RolePermission
{
    public class ActionItem
    {
        public string Code { get; set; }
        public string PersianName { get; set; }
        public string DisplayName => $"{PersianName} ({Code})";
    }
}
