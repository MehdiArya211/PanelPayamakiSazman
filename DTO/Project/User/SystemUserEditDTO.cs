using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO.Project.User
{
    public class SystemUserEditDTO
    {
        public string Id { get; set; }

        public string UserName { get; set; }
        public string FullName { get; set; }
        public string MobileNumber { get; set; }
        public string NationalCode { get; set; }
        public bool IsActive { get; set; }
    }
}
