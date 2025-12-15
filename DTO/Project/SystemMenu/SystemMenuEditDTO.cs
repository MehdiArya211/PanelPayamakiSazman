using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO.Project.SystemMenu
{
    public class SystemMenuEditDTO
    {
        public string Id { get; set; }
        public string Key { get; set; }
        public string Title { get; set; }
        public string Icon { get; set; }
        public string RouteKey { get; set; }
        public int Order { get; set; }
        public string ParentId { get; set; }
        public bool IsActive { get; set; }
    }
}
