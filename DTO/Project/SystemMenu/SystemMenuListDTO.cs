using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO.Project.SystemMenu
{
    public class SystemMenuListDTO
    {
        public string Id { get; set; }
        public string Key { get; set; }
        public string Title { get; set; }
        public int Order { get; set; }
        public string ParentId { get; set; }
        public string ParentTitle { get; set; }
        public bool IsActive { get; set; }
    }
}
