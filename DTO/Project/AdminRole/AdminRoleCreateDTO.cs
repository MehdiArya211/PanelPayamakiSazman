using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO.Project.AdminRole
{
    public class AdminRoleCreateDTO
    {
        [Required(ErrorMessage = "نام نقش الزامی است")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "نام نقش باید بین ۲ تا ۱۰۰ کاراکتر باشد")]
        [Display(Name = "نام نقش")]
        public string Name { get; set; }

        [StringLength(500, ErrorMessage = "توضیحات نمی‌تواند بیش از ۵۰۰ کاراکتر باشد")]
        [Display(Name = "توضیحات")]
        public string Description { get; set; }
    }
}
