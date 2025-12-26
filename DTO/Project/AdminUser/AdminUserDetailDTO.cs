using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO.Project.AdminUser
{
    /// <summary>
    /// جزئیات کامل کاربر مدیر
    /// </summary>
    public class AdminUserDetailDTO
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string NationalCode { get; set; }
        public string MobileNumber { get; set; }
        public string UnitId { get; set; }
        public List<AdminUserRoleDTO> Roles { get; set; }

        public string FullName => $"{FirstName} {LastName}";
    }
}
