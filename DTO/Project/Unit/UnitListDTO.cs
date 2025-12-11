using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO.Project.Unit
{
    /// <summary>
    /// مدل نمایشی واحد/گروه سازمانی در لیست‌ها
    /// </summary>
    public class UnitListDTO
    {
        public string Id { get; set; }          // GUID به صورت string
        public string Code { get; set; }        // code
        public string Name { get; set; }        // name
        public string? Description { get; set; } // description
        public bool IsActive { get; set; }      // isActive
        public string? ParentId { get; set; }   // parentId (ممکن است null باشد)
    }
}
