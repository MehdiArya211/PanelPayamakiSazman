using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO.Project.AdminRole
{
    public class AdminRoleEditDTO
    {
        [Required(ErrorMessage = "شناسه نقش الزامی است")]
        public string Id { get; set; }

        [Required(ErrorMessage = "نام نقش الزامی است")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "نام نقش باید بین ۲ تا ۱۰۰ کاراکتر باشد")]
        [Display(Name = "نام نقش")]
        public string Name { get; set; }

        [StringLength(500, ErrorMessage = "توضیحات نمی‌تواند بیش از ۵۰۰ کاراکتر باشد")]
        [Display(Name = "توضیحات")]
        public string Description { get; set; }

        [Display(Name = "فعال")]
        public bool IsActive { get; set; }

        // برای نمایش در فرم
        [Display(Name = "تاریخ ایجاد")]
        public DateTime CreatedAt { get; set; }

        [Display(Name = "ایجاد کننده")]
        public string CreatedBy { get; set; }

        [Display(Name = "تاریخ آخرین ویرایش")]
        public DateTime? UpdatedAt { get; set; }

        [Display(Name = "ویرایش کننده")]
        public string UpdatedBy { get; set; }
    }
}
