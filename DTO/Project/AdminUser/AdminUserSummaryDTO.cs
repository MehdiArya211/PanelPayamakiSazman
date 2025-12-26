using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO.Project.AdminUser
{
    /// <summary>
    /// خلاصه اطلاعات کاربر مدیر برای نمایش در لیست
    /// </summary>
    public class AdminUserSummaryDTO
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MobileNumber { get; set; }
        public string NationalCode { get; set; }
        public string UnitId { get; set; }
        public DateTime? LockoutEnd { get; set; }

        public string FullName => $"{FirstName} {LastName}";
        public bool IsLocked => LockoutEnd.HasValue && LockoutEnd > DateTime.UtcNow;
    }
}
