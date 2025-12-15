using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO.Project.Menus
{
    public class MenuListDTO
    {
        public string Id { get; set; }
        public string Key { get; set; }
        public string Title { get; set; }
        public string? Icon { get; set; }
        public string? RouteKey { get; set; }
        public int Order { get; set; }
        public string? ParentId { get; set; }
        public string? ParentTitle { get; set; } // عنوان والد
        public bool IsActive { get; set; }
    }
}
