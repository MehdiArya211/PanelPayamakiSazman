using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace DTO.User
{
    public class UserFilterDataTableDTO
    {

        [Display(Name = "نام کاربری")]
        public string Username { get; set; }


        [Display(Name = "نام")]
        public string Name { get; set; }


        [Display(Name = "موبایل")]
        public string Mobile { get; set; }


        [Display(Name = "شناسه نقش")]
        public long? RoleId { get; set; }


        [Display(Name = "فعال است")]
        public bool? IsEnabled { get; set; }


        [Display(Name = "تاریخ شروع")]
        public DateTime? StartDate { get; set; }


        [Display(Name = "تاریخ پایان")]
        public DateTime? EndDate { get; set; }

    }
}
