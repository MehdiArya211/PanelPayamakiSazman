using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO.Project.SenderNumberSubAreaList
{
    public class SenderNumberSubAreaEditDTO
    {
        public string Id { get; set; }   // از API برمی‌گردد

        public string Code { get; set; }
        public string Title { get; set; }
        public string? Description { get; set; }
    }

}
